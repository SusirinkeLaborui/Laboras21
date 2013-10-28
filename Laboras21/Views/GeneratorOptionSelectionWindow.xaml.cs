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
        public bool? Result { get; private set; }

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
            IRandomNumberGenerator generator;

            if (!ValidateCommonDistributionVariables())
            {
                return;
            }

            if (distributionComboBox.SelectedIndex == 0)
            {
                generator = new UniformRandomNumberGenerator();
                throw new NotImplementedException();        // Uniform generator constructor has to take in min/max values
            }
            else
            {
                if (!ValidateNormalDistributionVariables())
                {
                    return;
                }

                generator = new NormalRandomNumberGenerator(viewModel.StandardDeviation);
            }

            if (graphPoints.Capacity < viewModel.NumberOfPoints)
            {
                graphPoints.Capacity = viewModel.NumberOfPoints;
            }

            for (int i = 0; i < viewModel.NumberOfPoints; i++)
            {
                graphPoints.Add(generator.GeneratePoint());
            }

            int minx = graphPoints.Select(x => x.x).Min(),
                maxx = graphPoints.Select(x => x.x).Max(),
                miny = graphPoints.Select(x => x.y).Min(),
                maxy = graphPoints.Select(x => x.y).Max();

            double reduceX = ((double)(maxx - minx)) / (viewModel.MaxX - viewModel.MinX);
            double reduceY = ((double)(maxy - miny)) / (viewModel.MaxY - viewModel.MinY);

            for (int i = 0; i < graphPoints.Count; i++)
            {
                graphPoints[i] = new Point((int)(graphPoints[i].x / reduceX), (int)(graphPoints[i].y / reduceY));
            }

            Result = true;
            Close();
        }

        private bool ValidateCommonDistributionVariables()
        {            
            bool result = true;

            result &= !Validation.GetHasError(textBoxMinX);
            result &= !Validation.GetHasError(textBoxMaxX);
            result &= !Validation.GetHasError(textBoxMinY);
            result &= !Validation.GetHasError(textBoxMaxY);
            result &= !Validation.GetHasError(textBoxN);

            if (!result)
            {
                MessageBox.Show("One or more field has an invalid value!");
                return result;
            }

            result &= viewModel.MinX < viewModel.MaxX;
            if (!result)
            {
                MessageBox.Show("Minimum X has to be less than Maximum X!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return result;
            }

            result &= viewModel.MinX < viewModel.MaxX;
            if (!result)
            {
                MessageBox.Show("Minimum Y has to be less than Maximum Y!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return result;
            }

            return result;
        }

        private bool ValidateNormalDistributionVariables()
        {
            bool result = true;

            result &= !Validation.GetHasError(textBoxStandardDeviation);
            result &= !Validation.GetHasError(textBoxN);

            if (!result)
            {
                MessageBox.Show("One or more field has an invalid value!");
            }

            return result;
        }
    }
}
