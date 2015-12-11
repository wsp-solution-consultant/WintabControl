using System;
using System.Collections.Generic;
using System.Drawing;
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
using WintabDN;

namespace WintabControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CWintabContext _context = null;
        private CWintabData _data = null;
        bool enabled = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void showEnableButtonText()
        {
            if (enabled)
            {
                btnEnable.Content = "Disable";
            }
            else
            {
                btnEnable.Content = "Enable";
            }
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            showEnableButtonText();
            _context = OpenQueryDigitizerContext(true);

            if (System.Windows.Forms.SystemInformation.MonitorCount > 1)
            {
                this.Left = System.Windows.Forms.Screen.AllScreens[1].Bounds.Left + 
                    (System.Windows.Forms.Screen.AllScreens[1].Bounds.Width / 2) - 
                    (this.Width / 2);
            }
        }

        private CWintabContext OpenQueryDigitizerContext(bool enable)
        {
            bool status = false;
            CWintabContext logContext = null;

            try
            {
                logContext = CWintabInfo.GetDefaultDigitizingContext(ECTXOptionValues.CXO_MESSAGES);
                if (enable)
                {
                    logContext.Options |= (uint)ECTXOptionValues.CXO_SYSTEM;
                }

                if (logContext == null)
                {
                    System.Windows.Forms.MessageBox.Show("Failed to get digitizing context");
                }

                logContext.Name = "Context";

                WintabAxis tabletX = CWintabInfo.GetTabletAxis(EAxisDimension.AXIS_X);
                WintabAxis tabletY = CWintabInfo.GetTabletAxis(EAxisDimension.AXIS_Y);

                logContext.SysMode = false;
                logContext.InOrgX = 0;
                logContext.InOrgY = 0;
                logContext.InExtX = tabletX.axMax;
                logContext.InExtY = tabletY.axMax;


                // In Wintab, the tablet origin is lower left.  Move origin to upper left
                // so that it coincides with screen origin.
                //logContext.OutExtY = -logContext.OutExtY;

                status = logContext.Open();


            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

            return logContext;
        }

        private void CloseCurrentContext()
        {
            try
            {
                if (_context != null)
                {
                    _context.Close();
                    _context = null;
                    _data = null;
                }

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void btnEnable_Click(object sender, RoutedEventArgs e)
        {
            CloseCurrentContext();
            _context = OpenQueryDigitizerContext(!enabled);
            enabled = !enabled;
            showEnableButtonText();
            
        }
    }
}
