
using System.Windows;
using System.Windows.Controls;

namespace SkillProfi_WPFClient.Classes
{
    public static class BrowserBehavior
    {

        public static readonly DependencyProperty HideScrollProperty;
        public static readonly DependencyProperty HtmlStringProperty;

        static BrowserBehavior()
        {
            HideScrollProperty = DependencyProperty.RegisterAttached(
                "HideScroll",
                typeof(bool),
                typeof(BrowserBehavior),
                new FrameworkPropertyMetadata(false));

            HtmlStringProperty  = DependencyProperty.RegisterAttached(
                "HtmlString",
                typeof(string),
                typeof(BrowserBehavior),
                new FrameworkPropertyMetadata(OnHtmlStringChanged));
        }


        //new FrameworkPropertyMetadata(OnShowScrollChanged)

        [AttachedPropertyBrowsableForType(typeof(WebBrowser))]
        public static bool GetHideScroll(WebBrowser d)
        {
            return (bool)d.GetValue(HideScrollProperty);
        }
        public static void SetHideScroll(WebBrowser d, bool value)
        {
            d.SetValue(HideScrollProperty, value);
        }


        //[AttachedPropertyBrowsableForType(typeof(WebBrowser))]
        public static string GetHtmlString(WebBrowser d)
        {
            return (string)d.GetValue(HtmlStringProperty);
        }

        public static void SetHtmlString(WebBrowser d, string value)
        {
            d.SetValue(HtmlStringProperty, value);
        }

        static void OnHtmlStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WebBrowser wb)
            {
                bool showScroll = (bool)wb.GetValue(HideScrollProperty);
                //byte[] bytes = Encoding.Default.GetBytes(e.NewValue as string);
                //$"<!DOCTYPE html><html><head><meta charset = \"utf-8\"/></head><body>{{e.NewValue as string}}</body></html>"
                string doc = !showScroll ? $"<!DOCTYPE html><html><head><meta charset = \"utf-8\"/></head><bodystyle=\"text-align:justify; vertical-align:middle;\">{e.NewValue as string}</body></html>" :
                                          $"<!DOCTYPE html><html><head><meta charset = \"utf-8\"/></head><body scroll=\"no\" style=\"text-align:center;\">{e.NewValue as string}</body></html>";//
                wb.NavigateToString(doc);
            }
        }

    }
}
