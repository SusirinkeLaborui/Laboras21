using Laboras21.Controls;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
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

        public MainWindow()
        {
            InitializeComponent();
            AllowsTransparency = false;     // Fixes win32 window being invisible;

            treeFinder = new MinimalSpanningTreeFinder(canvas.AddEdgeAsync);

            VisualStateManager.GoToElementState(this, "StateInput", true);
        }

        private async void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            UberCanvas.ForwardInput = false;
            bool? userClickedOK = openFileDialog.ShowDialog();
            UberCanvas.ForwardInput = true;

            if (userClickedOK == true)
            {
                VisualStateManager.GoToElementState(this, "StateReadingFile", true);
                try
                {
                    graph = await DataProvider.ReadFromFileAsync(openFileDialog.FileName);
                }
                catch (BadFileFormatException exc)
                {
                    StyledMessageDialog.Show("The selected file is not valid: " + exc.Message, "Error");
                    VisualStateManager.GoToElementState(this, "StateInput", true);

                    return;
                }

                await canvas.SetCollectionAsync(graph);
                VisualStateManager.GoToElementState(this, "StateReadyToCompute", true);               
            }
        }
        
        private async void ButtonGenerate_Click(object sender, RoutedEventArgs e)
        {
            var generatorWindow = new GeneratorOptionSelectionWindow(graph, this);
            
            UberCanvas.ForwardInput = false;
            generatorWindow.ShowDialog();
            UberCanvas.ForwardInput = true;

            var dialogResult = generatorWindow.Result;
            if (!dialogResult.HasValue || !dialogResult.Value)
            {
                return;
            }
            
            try
            {
                VisualStateManager.GoToElementState(this, "StateGenerating", true);
                var generationTask = canvas.SetCollectionAsync(graph);
                await AskToSaveGeneratedData();
                await generationTask;
                VisualStateManager.GoToElementState(this, "StateReadyToCompute", true);
            }
            catch (OperationCanceledException)
            {
                VisualStateManager.GoToElementState(this, "StateInput", true);
            }
        }
        
        private async void ButtonStopGenerating_Click(object sender, RoutedEventArgs e)
        {
            await canvas.SetCollectionAsync(null);
        }

        private async void ButtonStartComputing_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToElementState(this, "StateComputing", true);
            await canvas.ClearEdgesAsync();
            await treeFinder.FindAsync(graph);
            await canvas.FinishDrawingAsync();
            VisualStateManager.GoToElementState(this, "StateDoneComputing", true);
        }

        private void ButtonStopComputing_Click(object sender, RoutedEventArgs e)
        {
            treeFinder.CancelSearch();
        }

        private async void ButtonSaveResults_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new SaveFileDialog();
            fileDialog.CheckPathExists = true;
            fileDialog.ValidateNames = true;
            fileDialog.OverwritePrompt = true;

            fileDialog.FileName = "Results.txt";
            fileDialog.DefaultExt = ".txt";
            fileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            UberCanvas.ForwardInput = false;
            bool? userClickedOK = fileDialog.ShowDialog();
            UberCanvas.ForwardInput = true;

            if (userClickedOK.HasValue && userClickedOK.Value)
            {
                VisualStateManager.GoToElementState(this, "StateWritingToFile", true);

                try
                {
                    await DataProvider.SaveResultsToFileAsync(fileDialog.FileName, graph);
                }
                catch (Exception ex)
                {
                    if (ex is UnauthorizedAccessException || ex is SecurityException)
                    {
                        StyledMessageDialog.Show("Insufficient rights to write to specified location.", "Error");
                    }
                    else if (ex is ArgumentException || ex is DirectoryNotFoundException)
                    {
                        StyledMessageDialog.Show("Invalid file was specified.", "Error");
                    }
                    else
                    {
                        throw;
                    }
                }

                VisualStateManager.GoToElementState(this, "StateDoneComputing", true);
            }
        }

        private async Task AskToSaveGeneratedData()
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

                    UberCanvas.ForwardInput = false;
                    var dialogResult = saveFileDialog.ShowDialog();
                    UberCanvas.ForwardInput = true;

                    if (dialogResult == true)
                    {
                        await DataProvider.SaveDataToFileAsync(saveFileDialog.FileName, graph);
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
