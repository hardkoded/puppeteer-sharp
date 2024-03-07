// * MIT License
//  *
//  * Copyright (c) Darío Kondratiuk
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  *
//  * The above copyright notice and this permission notice shall be included in all
//  * copies or substantial portions of the Software.
//  *
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  * SOFTWARE.

namespace PuppeteerSharp.Tests.TouchScreenTests;

internal record TouchEvent
{
    public string Type { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public decimal AltitudeAngle { get; set; }
    public decimal AzimuthAngle { get; set; }
    public decimal Pressure { get; set; }
    public decimal TiltX { get; set; }
    public decimal TiltY { get; set; }
    public int Twist { get; set; }
    public string PointerType { get; set; }
    public Detail[] ChangedTouches { get; set; } = [];

    public Detail[] ActiveTouches { get; set; } = [];

    internal record Detail
    {
        public int ClientX { get; set; }
        public int ClientY { get; set; }
        public decimal RadiusX { get; set; }
        public decimal RadiusY { get; set; }
        public decimal Force { get; set; }
    }
}
