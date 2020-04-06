﻿using SkiaChart.Axes;
using SkiaChart.Interfaces;
using SkiaChart.Models;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;

namespace SkiaChart.Charts {

    /// <summary>
    /// A class that handles the plotting of a single line chart on the ChartCanvas
    /// </summary>
    public class LineChart : ChartBase {

        /// <summary>
        /// Instantiates an instance of LineChart with floating point values
        /// </summary>
        /// <param name="xValues">X-Cordinates of the X-Y plot</param>
        /// <param name="yValues">Y-Cordinates of the X-Y plot</param>
        public LineChart(IEnumerable<float> xValues, IEnumerable<float> yValues) {
            ValidateInputs(xValues, yValues);
            UpdateDateType<float, float>();
            OriginalData = GenerateXYPoints(xValues, yValues);
        }

        /// <summary>
        /// Instantiates an instance of LineChart with X-Values as labels and YValues as floats
        /// </summary>
        /// <param name="xValues">X-Cordinates of the X-Y plot as string labels</param>
        /// <param name="yValues">Y-Cordinates of the X-Y plot</param>
        public LineChart(IEnumerable<string> xValues, IEnumerable<float> yValues) {
            ValidateInputs(xValues, yValues);
            UpdateDateType<string, float>();
            OriginalData = DistributeXGenerateYPoints(xValues, yValues);
        }

        /// <summary>
        /// Instantiates an instance of LineChart with Y-Values as labels and XValues as floats
        /// </summary>
        /// <param name="xValues">X-Cordinates of the X-Y plot</param>
        /// <param name="yValues">Y-Cordinates of the X-Y plot as string labels</param>
        public LineChart(IEnumerable<float> xValues, IEnumerable<string> yValues) {
            ValidateInputs(xValues, yValues);
            UpdateDateType<float, string>();
            OriginalData = DistributeYGenerateXPoints(xValues, yValues);
        }

        /// <summary>
        /// Renders a line chart a canvas. This method is called by the chart class.
        /// </summary>
        /// <param name="canvasWrapper">wrapper class containing info mation about the canvas and chart</param>
        /// <param name="axis">Axis orientation object</param>
        /// <param name="minMax">Data for the extreme values</param>
        /// <param name="gridPaint">Paint object for the grid lines</param>
        public override void RenderChart(CanvasWrapper canvasWrapper, Axis axis, IMinMax minMax) {
            CheckConstructionPolicy(nameof(LineChart));
            var canvas = canvasWrapper.Canvas;

            if (canvasWrapper.NumberPlottedChart < 1) {
                DrawHorizontalLabels(canvasWrapper, axis, minMax);
                DrawVerticalLabels(canvasWrapper, axis, minMax);
            }

            canvas.DrawPoints(SKPointMode.Lines, ConstructionData.ToArray(), _chartPaint);
            canvasWrapper.NumberPlottedChart += 1;

            if (canvasWrapper.CanShowLegend) {
                var start = (canvasWrapper.ChartArea.Bottom + 60);
                var end = start + (40f * canvasWrapper.NumberPlottedChart);
                canvas.Save();
                axis.AntiOrientAxis(float.MaxValue);
                canvas.DrawLine(canvasWrapper.ChartArea.Left, end, canvasWrapper.ChartArea.Left + 40, end, _chartPaint);
                canvas.Restore();
                axis.DrawAndPositionLegend(ChartName, end, canvasWrapper.ChartArea.Left + 40, _chartPaint);
            }
        }

        //Draws the vertical labels
        protected void DrawVerticalLabels(CanvasWrapper canvasWrapper, Axis axis, IMinMax minMax) {
            var heightSpacing = (canvasWrapper.ChartArea.Bottom - canvasWrapper.ChartArea.Top)
                                                / canvasWrapper.GridLines;
            var heightHolder = heightSpacing;
            heightSpacing += canvasWrapper.Converter.YOffset;
            for (int i = 0; i < canvasWrapper.GridLines; i++) {
                var labelValue = canvasWrapper.Converter
                                              .YValueToRealScale(heightSpacing, minMax.Ymax, minMax.Ymin);
                axis.DrawAndPositionYTickMark(GetYLabel(labelValue), heightSpacing, canvasWrapper.ChartArea.Left, _labelPaint);
                heightSpacing += heightHolder;
            }
        }

        //Draws the horizontal labels
        protected void DrawHorizontalLabels(CanvasWrapper canvasWrapper, Axis axis, IMinMax minMax) {

            var widthSpacing = (canvasWrapper.ChartArea.Right - canvasWrapper.ChartArea.Left)
                                                    / canvasWrapper.GridLines;
            var widthHolder = widthSpacing;
            widthSpacing += canvasWrapper.Converter.XOffset;
            for (int i = 0; i < canvasWrapper.GridLines; i++) {
                var labelValue = canvasWrapper.Converter
                                          .XValueToRealScale(widthSpacing, minMax.Xmax, minMax.Xmin);
                axis.DrawAndPositionXTickMark(GetXLabel(labelValue), widthSpacing, canvasWrapper.ChartArea.Bottom, _labelPaint);
                widthSpacing += widthHolder;
            }

        }

        private SKColor _labelColor;
        /// <summary>
        ///  Color of Label
        /// </summary>
        public SKColor LabelColor {
            get => _labelColor;
            set {
                if (value != _labelColor) {
                    _labelColor = value;
                    _labelPaint.Color = value;
                }
            }
        }

        protected readonly SKPaint _labelPaint = new SKPaint() {
            Style = SKPaintStyle.Stroke,
            IsAntialias = true,
            StrokeWidth = 3,
            Color = SKColors.Gray
        };
    }
}
