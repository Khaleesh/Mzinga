﻿// 
// MainWindow.xaml.cs
//  
// Author:
//       Jon Thysell <thysell@gmail.com>
// 
// Copyright (c) 2015 Jon Thysell <http://jonthysell.com>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

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

using Mzinga.Core;
using Mzinga.Viewer.ViewModel;

namespace Mzinga.Viewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double HexRadiusRatio = 0.05;

        private string LastBoardString;

        private SolidColorBrush WhiteBrush;
        private SolidColorBrush BlackBrush;
        private SolidColorBrush HighlightBrush;

        private SolidColorBrush QueenBeeBrush;
        private SolidColorBrush SpiderBrush;
        private SolidColorBrush BeetleBrush;
        private SolidColorBrush GrasshopperBrush;
        private SolidColorBrush SoldierAntBrush;

        public MainWindow()
        {
            InitializeComponent();

            // Init brushes
            WhiteBrush = new SolidColorBrush(Colors.White);
            BlackBrush = new SolidColorBrush(Colors.Black);
            HighlightBrush = new SolidColorBrush(Colors.Aqua);

            QueenBeeBrush = new SolidColorBrush(Colors.Gold);
            SpiderBrush = new SolidColorBrush(Colors.Brown);
            BeetleBrush = new SolidColorBrush(Colors.Purple);
            GrasshopperBrush = new SolidColorBrush(Colors.Green);
            SoldierAntBrush = new SolidColorBrush(Colors.Blue);

            // Bind board updates to VM
            MainViewModel vm = DataContext as MainViewModel;
            if (null != vm)
            {
                vm.BoardUpdated += DrawBoard;
            }
        }

        private void DrawBoard(string boardString)
        {
            BoardCanvas.Children.Clear();

            if (!String.IsNullOrWhiteSpace(boardString))
            {
                double minX = Double.MaxValue;
                double minY = Double.MaxValue;
                double maxX = Double.MinValue;
                double maxY = Double.MinValue;

                int maxStack;
                int numPieces;
                Dictionary<int, List<Piece>> pieces = GetPieces(boardString, out numPieces, out maxStack);

                double size = Math.Min(HexRadiusRatio, (double)numPieces / (double)EnumUtils.NumPieceNames) * Math.Min(BoardCanvas.ActualHeight, BoardCanvas.ActualWidth);

                for (int stack = 0; stack <= maxStack; stack++)
                {
                    if (pieces.ContainsKey(stack))
                    {
                        foreach (Piece piece in pieces[stack])
                        {
                            double centerX = size * 1.5 * piece.Position.Q;
                            double centerY = size * Math.Sqrt(3.0) * (piece.Position.R + (0.5 * piece.Position.Q));

                            HexType hexType = (piece.Color == Core.Color.White) ? HexType.White : HexType.Black;

                            Polygon hex = GetHex(centerX, centerY, size, hexType);
                            BoardCanvas.Children.Add(hex);

                            TextBlock hexText = GetHexText(centerX, centerY, size, piece.PieceName);
                            BoardCanvas.Children.Add(hexText);

                            minX = Math.Min(minX, centerX - size);
                            minY = Math.Min(minY, centerY - size);

                            maxX = Math.Max(maxX, centerX + size);
                            maxY = Math.Max(maxY, centerY + size);
                        }

                        // Translate board
                        double boardWidth = Math.Abs(maxX - minX);
                        double boardHeight = Math.Abs(maxY - minY);

                        double boardCenterX = minX + (boardWidth / 2);
                        double boardCenterY = minY + (boardHeight / 2);

                        double canvasCenterX = BoardCanvas.ActualWidth / 2;
                        double canvasCenterY = BoardCanvas.ActualHeight / 2;

                        double offsetX = canvasCenterX - boardCenterX;
                        double offsetY = canvasCenterY - boardCenterY;

                        BoardCanvas.RenderTransform = new TranslateTransform(offsetX, offsetY);
                    }
                }
            }

            LastBoardString = boardString;
        }

        private Dictionary<int, List<Piece>> GetPieces(string boardString, out int numPieces, out int maxStack)
        {
            if (String.IsNullOrWhiteSpace(boardString))
            {
                throw new ArgumentNullException("boardString");
            }

            numPieces = 0;
            maxStack = -1;

            Dictionary<int, List<Piece>> pieces = new Dictionary<int, List<Piece>>();

            string[] split = boardString.Split(Board.BoardStringSeparator);
            for (int i = 2; i < split.Length; i++)
            {
                Piece piece = new Piece(split[i]);

                int stack = piece.Position.Stack;
                maxStack = Math.Max(maxStack, stack);

                if (!pieces.ContainsKey(stack))
                {
                    pieces[stack] = new List<Piece>();
                }

                pieces[stack].Add(piece);
                numPieces++;
            }

            return pieces;
        }

        private Polygon GetHex(double centerX, double centerY, double size, HexType hexType)
        {
            Polygon hex = new Polygon();
            hex.Stroke = BlackBrush;
            hex.StrokeThickness = 2;

            switch (hexType)
            {
                case HexType.White:
                    hex.Fill = WhiteBrush;
                    break;
                case HexType.WhiteHighlighted:
                    hex.Fill = WhiteBrush;
                    hex.Stroke = HighlightBrush;
                    break;
                case HexType.Black:
                    hex.Fill = BlackBrush;
                    break;
                case HexType.BlackHighlighted:
                    hex.Fill = BlackBrush;
                    hex.Stroke = HighlightBrush;
                    break;
                case HexType.ValidMove:
                    hex.Stroke = HighlightBrush;
                    break;
            }

            PointCollection points = new PointCollection();

            for (int i = 0; i < 6; i++)
            {
                double angle_deg = 60.0 * i;
                double angle_rad = Math.PI / 180 * angle_deg;
                points.Add(new Point(centerX + size * Math.Cos(angle_rad), centerY + size * Math.Sin(angle_rad)));
            }

            hex.Points = points;

            return hex;
        }

        private TextBlock GetHexText(double centerX, double centerY, double size, Core.PieceName pieceName)
        {
            TextBlock hexText = new TextBlock();
            hexText.Text = EnumUtils.GetShortName(pieceName).Substring(1);
            hexText.FontFamily = new FontFamily("Lucida Console");

            switch (pieceName)
            {
                case PieceName.WhiteQueenBee:
                case PieceName.BlackQueenBee:
                    hexText.Foreground = QueenBeeBrush;
                    break;
                case PieceName.WhiteSpider1:
                case PieceName.WhiteSpider2:
                case PieceName.BlackSpider1:
                case PieceName.BlackSpider2:
                    hexText.Foreground = SpiderBrush;
                    break;
                case PieceName.WhiteBeetle1:
                case PieceName.WhiteBeetle2:
                case PieceName.BlackBeetle1:
                case PieceName.BlackBeetle2:
                    hexText.Foreground = BeetleBrush;
                    break;
                case PieceName.WhiteGrasshopper1:
                case PieceName.WhiteGrasshopper2:
                case PieceName.WhiteGrassHopper3:
                case PieceName.BlackGrasshopper1:
                case PieceName.BlackGrasshopper2:
                case PieceName.BlackGrassHopper3:
                    hexText.Foreground = GrasshopperBrush;
                    break;
                case PieceName.WhiteSoldierAnt1:
                case PieceName.WhiteSoldierAnt2:
                case PieceName.WhiteSoldierAnt3:
                case PieceName.BlackSoldierAnt1:
                case PieceName.BlackSoldierAnt2:
                case PieceName.BlackSoldierAnt3:
                    hexText.Foreground = SoldierAntBrush;
                    break;
            }

            hexText.FontSize = size;

            Canvas.SetLeft(hexText, centerX - (hexText.Text.Length * (hexText.FontSize / 3.5)));
            Canvas.SetTop(hexText, centerY - (hexText.FontSize / 2.0));

            return hexText;
        }

        private enum HexType
        {
            White,
            Black,
            WhiteHighlighted,
            BlackHighlighted,
            ValidMove
        }

        private void Board_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawBoard(LastBoardString);
        }
    }
}
