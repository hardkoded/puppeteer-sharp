using System.Diagnostics;
using System.IO;
using System.Windows;

namespace CefSharp.Puppeteer.Wpf.Example
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Browser.RegisterResourceHandler("https://cefsharp.test/textarea.html", File.OpenRead("textarea.html"));
        }

        private async void ExecuteTestButtonClick(object sender, RoutedEventArgs e)
        {
            await Browser.WaitForInitialLoadAsync();

            var devToolsContext = await Browser.CreateDevToolsContextAsync(ignoreHTTPSerrors:true);

            await devToolsContext.GoToAsync("https://cefsharp.test/textarea.html");

            await devToolsContext.EvaluateExpressionAsync(@"{
              window.addEventListener('keydown', event => window.keyLocation = event.location, true);
            }");
            var textarea = await devToolsContext.QuerySelectorAsync("textarea");

            await textarea.PressAsync("Digit5");

            var actual = await devToolsContext.EvaluateExpressionAsync<int>("keyLocation");

            Debug.Assert(0 == actual);

            await textarea.PressAsync("ControlLeft");

            actual = await devToolsContext.EvaluateExpressionAsync<int>("keyLocation");

            Debug.Assert(1 == actual);

            await textarea.PressAsync("ControlRight");

            actual = await devToolsContext.EvaluateExpressionAsync<int>("keyLocation");

            Debug.Assert(2 == actual);

            await textarea.PressAsync("NumpadSubtract");

            actual = await devToolsContext.EvaluateExpressionAsync<int>("keyLocation");

            Debug.Assert(3 == actual);

            if(actual == 3)
            {
                await textarea.SetPropertyValueAsync("value", "Testing Complete");
            }

            await devToolsContext.DisposeAsync();
        }

        private void ShowDevToolsClick(object sender, RoutedEventArgs e)
        {
            Browser.ShowDevTools();
        }
    }
}
