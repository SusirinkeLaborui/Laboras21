using Laboras21.Controls;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Laboras21.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private List<Vertex> graph = new List<Vertex>();
        private MinimalSpanningTreeFinder treeFinder;
        SuperCanvas canvas = new SuperCanvas();

        public MainWindow()
        {
            InitializeComponent();
            AllowsTransparency = false;     // Fixes win32 window being invisible

            Action<double> reportProgressCallback = (progress) => progressBar.Dispatcher.InvokeAsync(() =>
                {
                    progressBar.Value = progress;
                }, DispatcherPriority.Background);

            treeFinder = new MinimalSpanningTreeFinder(canvas.AddEdge, reportProgressCallback);
            canvas.ProgressBar = progressBar;
            VisualStateManager.GoToElementState(this.LayoutRoot, "StateInput", true);
        }

        private void InitD3D()
        {
            var d3DWindow = new D3DWindow(LayoutRoot.ActualWidth - 300, LayoutRoot.ActualHeight);
            LayoutRoot.Children.Add(d3DWindow);
        }

        private async void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            InitD3D();
            var openFileDialog = new OpenFileDialog();
            bool? userClickedOK = openFileDialog.ShowDialog();

            if (userClickedOK == true)
            {             
                VisualStateManager.GoToElementState(this.LayoutRoot, "StateReadingFile", true);
                try
                {
                    graph = await DataProvider.ReadFromFileAsync(openFileDialog.FileName);
                }
                catch (BadFileFormatException exc)
                {
                    StyledMessageDialog.Show("The selected file is not valid: " + exc.Message, "Error");
                    VisualStateManager.GoToElementState(this.LayoutRoot, "StateInput", true);

                    return;
                }

                await canvas.SetCollectionAsync(graph);
                VisualStateManager.GoToElementState(this.LayoutRoot, "StateReadyToCompute", true);               
            }
        }
        
        private async void ButtonGenerate_Click(object sender, RoutedEventArgs e)
        {
            var generatorWindow = new GeneratorOptionSelectionWindow(graph, this);
            
            generatorWindow.ShowDialog();
            var dialogResult = generatorWindow.Result;
            if (!dialogResult.HasValue || !dialogResult.Value)
            {
                return;
            }
            
            progressBar.Value = 0;
            try
            {
                VisualStateManager.GoToElementState(this.LayoutRoot, "StateGenerating", true);
                var generationTask = canvas.SetCollectionAsync(graph);
                AskToSaveGeneratedData();
                await generationTask;
                VisualStateManager.GoToElementState(this.LayoutRoot, "StateReadyToCompute", true);
            }
            catch (OperationCanceledException)
            {
                VisualStateManager.GoToElementState(this.LayoutRoot, "StateInput", true);
            }
        }
        
        private async void ButtonStopGenerating_Click(object sender, RoutedEventArgs e)
        {
            await canvas.SetCollectionAsync(null);
        }

        private async void ButtonStartComputing_Click(object sender, RoutedEventArgs e)
        {
            progressBar.Value = 0;

            VisualStateManager.GoToElementState(this.LayoutRoot, "StateComputing", true);
            await canvas.ClearEdgesAsync();
            await treeFinder.FindAsync(graph);
            await canvas.FinishDrawingAsync();
            VisualStateManager.GoToElementState(this.LayoutRoot, "StateDoneComputing", true);
        }

        private void ButtonStopComputing_Click(object sender, RoutedEventArgs e)
        {
            treeFinder.CancelSearch();
            canvas.CancelDrawing();
        }

        private void ButtonSaveResults_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AskToSaveGeneratedData()
        {
            bool showDialog = true;
            while (showDialog)
            {
                var saveToFileDialogResult = StyledMessageDialog.Show("Save generated results to file?", "Save to file", MessageBoxButton.YesNo);
                if (saveToFileDialogResult.HasValue && saveToFileDialogResult.Value)
                {
                    var saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = "untitled.txt";
                    saveFileDialog.Filter = "Text File (*.txt)|*.*";
                    saveFileDialog.Title = "Save as";
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        DataProvider.SaveDataToFileAsync(saveFileDialog.FileName, graph);
                        showDialog = false;
                    }
                    else
                    {
                        showDialog = true;
                    }
                }
                else
                {
                    showDialog = false;
                }
            }
        }

        private async void Sing()
        {
            await Task.Run(() =>
                {
                    var synth = new System.Speech.Synthesis.SpeechSynthesizer();
                    synth.SetOutputToDefaultAudioDevice();

                    System.Speech.Synthesis.VoiceGender[] genders = 
                    { 
                         System.Speech.Synthesis.VoiceGender.Male,
                         System.Speech.Synthesis.VoiceGender.Female
                    };

                    var poemToPlay = Poem1;

                    for (int i = 0; i < poemToPlay.Length; i++)
                    {
                        synth.SelectVoiceByHints(genders[i % genders.Length]);
                        synth.Speak(poemToPlay[i]);
                    }
                });
        }

        readonly string[] Poem1 =
        {
            @"The Owl and the Pussy Cat went to sea In a beautiful pea-green boat,
            They took some honey and plenty of money Wrapped up in a five-pound note.
            The Owl looked up to the stars above and sang to a small guitar:",

            @"O lovely Pussy O Pussy my love,
            What a beautiful Pussy you are, You are You are!
            What a beautiful Pussy you are!",

            @"Pussy said to the Owl, ",
            
            @"You elegant fowl How charmingly sweet you sing!
            O let us be married too long we have tarried:
            But what shall we do for a ring?",

            @"They sailed away for a year and a day To the land where the Bong-tree grows
            And there in a wood a Piggy-wig stood With a ring at the end of his nose, His nose His nose,
            With a ring at the end of his nose.",

            @"Dear Pig, are you willing to sell for one shilling Your ring?",
            
            @"Said the Piggy, ",
            
            @"I will.",
            
            @"So they took it away, and were married next day By the Turkey who lives on the hill.
            They dined on mince and slices of quince, Which they ate with a runcible spoon;
            And hand in hand, on the edge of the sand,
            They danced by the light of the moon, The moon The moon,
            They danced by the light of the moon."
        };
    }
}
