using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ResNet_Image_ClassificationSample
{
    internal class ImageNet
    {
        public static Prediction[] GetSoftmax(IEnumerable<float> output)
        {
            float sum = output.Sum(x => (float)Math.Exp(x));
            IEnumerable<float> softmax = output.Select(x => (float)Math.Exp(x) / sum);

            return softmax.Select((x, i) => new Prediction { Label = ImageNetLabels.Labels[i], Confidence = float.Parse(x.ToString("F4", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture) })
                               .OrderByDescending(x => x.Confidence)
                               .Take(5)
                               .ToArray();
        }

        public static void DisplayPredictions(IEnumerable<Prediction> predictions, StackPanel PredictionsStackPanel)
        {
            // Clear previous predictions
            PredictionsStackPanel.Children.Clear();

            // Set headers
            Grid headerRow = new Grid
            {
                Margin = new Thickness(0, 8, 0, 8)
            };
            headerRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            headerRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerRow.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

            // Create a TextBlock for the label
            TextBlock labelHeader = new TextBlock
            {
                Text = "Label",
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 8, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(labelHeader, 0);

            // Create a TextBlock for the confidence
            TextBlock confidenceHeader = new TextBlock
            {
                Text = "Confidence",
                FontSize = 14,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 8, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(confidenceHeader, 2);

            headerRow.Children.Add(labelHeader);
            headerRow.Children.Add(confidenceHeader);

            PredictionsStackPanel.Children.Add(headerRow);

            foreach (var prediction in predictions)
            {
                // Create a Grid to hold the label and the progress bar
                Grid predictionGrid = new Grid
                {
                    Margin = new Thickness(0, 8, 0, 8)
                };
                predictionGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                predictionGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                predictionGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

                // Create a TextBlock for the label
                TextBlock labelTextBlock = new TextBlock
                {
                    Text = prediction.Label,
                    FontSize = 14,
                    Margin = new Thickness(0, 0, 8, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(labelTextBlock, 0);

                // Create a TextBlock for the percentage
                TextBlock confidenceTextBlock = new TextBlock
                {
                    Text = $"{prediction.Confidence * 100} %",
                    FontSize = 14,
                    Margin = new Thickness(5, 0, 8, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(confidenceTextBlock, 2);

                predictionGrid.Children.Add(labelTextBlock);
                predictionGrid.Children.Add(confidenceTextBlock);

                PredictionsStackPanel.Children.Add(predictionGrid);
            }
        }
    }
}