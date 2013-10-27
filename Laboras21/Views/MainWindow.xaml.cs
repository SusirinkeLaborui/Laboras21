using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Laboras21
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Point> graphPoints = new List<Point>();

        public MainWindow()
        {
            InitializeComponent();

            VisualStateManager.GoToElementState(this.LayoutRoot, "StateInput", true);
        }

        private void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToElementState(this.LayoutRoot, "StateReadyToCompute", true);

            Sing();
        }

        private void ButtonGenerate_Click(object sender, RoutedEventArgs e)
        {
            var generatorWindow = new GeneratorOptionSelectionWindow(graphPoints);

            if (generatorWindow.ShowDialog() == false)
            {
                return;
            }

            VisualStateManager.GoToElementState(this.LayoutRoot, "StateReadyToCompute", true);
        }

        private void ButtonStartComputing_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToElementState(this.LayoutRoot, "StateComputing", true);
        }

        private void ButtonStopComputing_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToElementState(this.LayoutRoot, "StateReadyToCompute", true);
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
