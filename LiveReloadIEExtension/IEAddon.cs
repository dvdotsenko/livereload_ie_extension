using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SHDocVw;
using mshtml;
using Microsoft.Win32;
using System.Windows.Forms;

namespace LiveReload {
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("D8FAF7A1-1D88-11E2-AE2C-001C230C8ABD")]
    [ProgId("LiveReload.IEExtension")]
    public class IEExtension : IObjectWithSite, IOleCommandTarget {
        IWebBrowser2 browser;

        #region
        public void InjectScriptResource(IHTMLWindow2 window, string resourcepath) {
            // Resource's path is **Default Namespace** (from project properties) + '.' + file name.
            // see http://www.codeproject.com/KB/dotnet/embeddedresources.aspx
            // and http://www.jelovic.com/articles/resources_in_visual_studio.htm
            string DefaultNamespace = "LiveReload.scripts.";
            using (Stream script = typeof(IEExtension).Assembly.GetManifestResourceStream(DefaultNamespace + resourcepath)) {
                if (script == null) {
                    throw new Exception("Could not read resource '" + resourcepath + "'");
                }
                window.execScript((new StreamReader(script)).ReadToEnd());
            }
        }

        public bool IsSupportedURL(string url) {
            // The following host name patterns are supported:

            // - protocol is one of: "http" or "https" AND ONE OF BELOW:
            //  - host name is "localhost"
            //  - host name starts with "test." (last subdomain is 'test')
            //  - the host is serving on port higher than 1024 (unlikely to be public server, but allows all IP addresses as hostname.

            try {
                System.Uri uriobj = new System.Uri(url);
                if (uriobj.Scheme == System.Uri.UriSchemeHttp || uriobj.Scheme == System.Uri.UriSchemeHttps) { } else { return false; }
                if (uriobj.IsLoopback) { return true; }
                if (uriobj.Host.StartsWith("test.")) { return true; }
                if (uriobj.Port > 1024) { return true; }
            } catch (Exception) {}
            return false;
        }

        public void InsertLiveReloadAndFriends() {
            if (browser!= null && IsSupportedURL(browser.LocationURL)) {
                var document = browser.Document as IHTMLDocument2;
                var window = document.parentWindow;
                try {
                    InjectScriptResource(window, "one_script_to_rule_them_all.js");
                } catch (Exception ex) {
                    window.alert("LiveReload encountered an error: " + ex.Message + " " + ex.StackTrace);
                }
            }
        }

        public void OnDownloadComplete() {
            InsertLiveReloadAndFriends();
        }
        public void OnDocumentComplete(object pDisp, ref object URL) {
            InsertLiveReloadAndFriends();
        }
        #endregion

        #region Implementation of IObjectWithSite
        int IObjectWithSite.SetSite(object site) {
            if (site != null) {
                browser = (IWebBrowser2)site;
                ((DWebBrowserEvents2_Event)browser).DocumentComplete +=
                    new DWebBrowserEvents2_DocumentCompleteEventHandler(this.OnDocumentComplete);
                ((DWebBrowserEvents2_Event)browser).DownloadComplete +=
                    new DWebBrowserEvents2_DownloadCompleteEventHandler(this.OnDownloadComplete);
            } else {
                ((DWebBrowserEvents2_Event)browser).DocumentComplete -=
                    new DWebBrowserEvents2_DocumentCompleteEventHandler(this.OnDocumentComplete);
                ((DWebBrowserEvents2_Event)browser).DownloadComplete -=
                    new DWebBrowserEvents2_DownloadCompleteEventHandler(this.OnDownloadComplete);
                browser = null;
            }
            return 0;
        }
        int IObjectWithSite.GetSite(ref Guid guid, out IntPtr ppvSite) {
            IntPtr punk = Marshal.GetIUnknownForObject(browser);
            int hr = Marshal.QueryInterface(punk, ref guid, out ppvSite);
            Marshal.Release(punk);
            return hr;
        }
        #endregion
        #region Implementation of IOleCommandTarget
        int IOleCommandTarget.QueryStatus(IntPtr pguidCmdGroup, uint cCmds, ref OLECMD prgCmds, IntPtr pCmdText) {
            return 0;
        }
        int IOleCommandTarget.Exec(IntPtr pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut) {
            return 0;
        }
        #endregion

        #region Registering with regasm
        public static string RegBHO = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Browser Helper Objects";
        // public static string RegCmd = "Software\\Microsoft\\Internet Explorer\\Extensions";

        [ComRegisterFunction]
        public static void RegisterBHO(Type type) {
            string guid = type.GUID.ToString("B");

            // BHO
            {
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(RegBHO, true);
                if (registryKey == null)
                    registryKey = Registry.LocalMachine.CreateSubKey(RegBHO);
                RegistryKey key = registryKey.OpenSubKey(guid);
                if (key == null)
                    key = registryKey.CreateSubKey(guid);
                key.SetValue("Alright", 1);
                registryKey.Close();
                key.Close();
            }

            //// Command
            //{
            //    RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(RegCmd, true);
            //    if (registryKey == null)
            //        registryKey = Registry.LocalMachine.CreateSubKey(RegCmd);
            //    RegistryKey key = registryKey.OpenSubKey(guid);
            //    if (key == null)
            //        key = registryKey.CreateSubKey(guid);
            //    key.SetValue("ButtonText", "Highlighter options");
            //    key.SetValue("CLSID", "{1FBA04EE-3024-11d2-8F1F-0000F87ABD16}");
            //    key.SetValue("ClsidExtension", guid);
            //    key.SetValue("Icon", "");
            //    key.SetValue("HotIcon", "");
            //    key.SetValue("Default Visible", "Yes");
            //    key.SetValue("MenuText", "&Highlighter options");
            //    key.SetValue("ToolTip", "Highlighter options");
            //    //key.SetValue("KeyPath", "no");
            //    registryKey.Close();
            //    key.Close();
            //}
        }

        [ComUnregisterFunction]
        public static void UnregisterBHO(Type type) {
            string guid = type.GUID.ToString("B");
            // BHO
            {
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(RegBHO, true);
                if (registryKey != null)
                    registryKey.DeleteSubKey(guid, false);
            }
            // Command
            //{
            //    RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(RegCmd, true);
            //    if (registryKey != null)
            //        registryKey.DeleteSubKey(guid, false);
            //}
        }
        #endregion
    }
}