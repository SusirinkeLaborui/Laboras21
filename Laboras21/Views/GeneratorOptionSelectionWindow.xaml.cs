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
using System.Windows.Shapes;

namespace Laboras21
{
    /// <summary>
    /// Interaction logic for GeneratorOptionSelectionWindow.xaml
    /// </summary>
    public partial class GeneratorOptionSelectionWindow : Window
    {
        List<Point> graphPoints;
        bool result = false;
        GeneratorOptionViewModel viewModel;

        public GeneratorOptionSelectionWindow(List<Point> graphPoints)
        {
            this.graphPoints = graphPoints;

            InitializeComponent();

            viewModel = new GeneratorOptionViewModel();
            DataContext = viewModel;
            DistributionComboBox_SelectionChanged(this, null);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            DialogResult = result;

            base.OnClosing(e);
        }

        private void DistributionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VisualStateManager.GoToElementState(this.LayoutRoot, 
                "State" + (distributionComboBox.SelectedItem as ComboBoxItem).Content.ToString() + "Distribution", true);
        }

        private void ButtonOk_Click(object sender, RoutedEventArgs e)
        {
            if (distributionComboBox.SelectedIndex == 0)
            {
                if (!ValidateUniformDistributionVariables())
                {
                    return;
                }

                var uniformGenerator = new UniformRandomNumeberGenerator();
                throw new NotImplementedException();
            }
            else
            {
                if (!ValidateNormalDistributionVariables())
                {
                    return;
                }

                var normalGenerator = new NormalRandomNumberGenerator(viewModel.StandardDeviation);
                throw new NotImplementedException();
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        private bool ValidateUniformDistributionVariables()
        {            
            bool result = true;

            result &= !Validation.GetHasError(textBoxMinX);
            result &= !Validation.GetHasError(textBoxMaxX);
            result &= !Validation.GetHasError(textBoxMinY);
            result &= !Validation.GetHasError(textBoxMaxY);

            if (result)
            {
                result &= viewModel.MinX < viewModel.MaxX;

                if (!result)
                {
                    MessageBox.Show("Minimum X has to be less than Maximum X!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            if (result)
            {
                result &= viewModel.MinX < viewModel.MaxX;

                if (!result)
                {
                    MessageBox.Show("Minimum Y has to be less than Maximum Y!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return result;
        }

        private bool ValidateNormalDistributionVariables()
        {
            bool result = true;

            result &= !Validation.GetHasError(textBoxStandardDeviation);

            return result;
        }
    }
}
