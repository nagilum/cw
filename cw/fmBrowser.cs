using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace cw {
    public partial class fmBrowser : Form {
        private string url { get; }
        private bool saveCache { get; }

        public fmBrowser(string url, bool saveCache) {
            InitializeComponent();

            this.url = url;
            this.saveCache = saveCache;
        }

        private void fmBrowser_Load(object sender, EventArgs e) {
#pragma warning disable 618
            var cwb = new ChromiumWebBrowser {
#pragma warning restore 618
                DisplayHandler = new CWB_DisplayHandler(),
                Dock = DockStyle.Fill
            };

            this.Controls.Add(cwb);

            if (this.saveCache) {
                // TODO: Enable cache saving!
            }

            cwb.Load(this.url);
        }
    }

    public class CWB_DisplayHandler : IDisplayHandler {
        public void OnAddressChanged(IWebBrowser browserControl, AddressChangedEventArgs addressChangedArgs) {}

        public void OnTitleChanged(IWebBrowser browserControl, TitleChangedEventArgs titleChangedArgs) {}

        public void OnFaviconUrlChange(IWebBrowser browserControl, IBrowser browser, IList<string> urls) {}

        public void OnFullscreenModeChange(IWebBrowser browserControl, IBrowser browser, bool fullscreen) {}

        public bool OnTooltipChanged(IWebBrowser browserControl, string text) {
            return false;
        }

        public void OnStatusMessage(IWebBrowser browserControl, StatusMessageEventArgs statusMessageArgs) {}

        public bool OnConsoleMessage(IWebBrowser browserControl, ConsoleMessageEventArgs consoleMessageArgs) {
            return false;
        }
    }
}