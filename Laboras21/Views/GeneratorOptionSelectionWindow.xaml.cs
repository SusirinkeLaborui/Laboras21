using Laboras21.Generators;
using Laboras21.ViewModels;
using MahApps.Metro.Controls;
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

namespace Laboras21.Views
{
    /// <summary>
    /// Interaction logic for GeneratorOptionSelectionWindow.xaml
    /// </summary>
    public partial class GeneratorOptionSelectionWindow : MetroWindow
    {
        List<Vertex> graph;
        bool result = false;
        GeneratorOptionViewModel viewModel;
        public bool? Result { get; private set; }

        public GeneratorOptionSelectionWindow(List<Vertex> graph)
        {
            this.graph = graph;

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
                generator = new UniformRandomNumberGenerator(viewModel.MinX, viewModel.MaxX, viewModel.MinY, viewModel.MaxY);
            }
            else
            {
                if (!ValidateNormalDistributionVariables())
                {
                    return;
                }

                generator = new NormalRandomNumberGenerator(viewModel.StandardDeviation, viewModel.MinX, viewModel.MaxX, viewModel.MinY, viewModel.MaxY);
            }

            graph.Clear();
            if (graph.Capacity < viewModel.NumberOfPoints)
            {
                graph.Capacity = viewModel.NumberOfPoints;
            }

            for (int i = 0; i < viewModel.NumberOfPoints; i++)
            {
                graph.Add(new Vertex(generator.GeneratePoint()));
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
                StyledMessageDialog.Show("One or more fields has an invalid value!", "Error");
                return result;
            }

            result &= viewModel.MinX < viewModel.MaxX;
            if (!result)
            {
                StyledMessageDialog.Show("Minimum X has to be less than Maximum X!", "Error");
                return result;
            }

            result &= viewModel.MinY < viewModel.MaxY;
            if (!result)
            {
                StyledMessageDialog.Show("Minimum Y has to be less than Maximum Y!", "Error");
                return result;
            }

            return result;
        }

        private bool ValidateNormalDistributionVariables()
        {
            bool result = true;

            result &= !Validation.GetHasError(textBoxStandardDeviation);

            if (!result)
            {
                StyledMessageDialog.Show("One or more fields has an invalid value!", "Error");
            }

            return result;
        }
    }
}
