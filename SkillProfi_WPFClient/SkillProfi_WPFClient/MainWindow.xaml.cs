using SkillProfi_WPFClient.Classes;
using SkillProfi_WPFClient.ViewModels;
using System.Windows;

namespace SkillProfi_WPFClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SkillProfiViewModel context = new (new DefaultDialogService());
            this.DataContext = context;
            this.pbCurrentAuthorizationPassword.PasswordChanged += delegate { context.CurrentAuthorizationPassword = pbCurrentAuthorizationPassword.Password; };
            this.pbAddUserPassword.PasswordChanged += delegate { context.AddUserPassword = pbAddUserPassword.Password; };
            this.pbAddUserConfirmPassword.PasswordChanged += delegate { context.AddUserConfirmPassword = pbAddUserConfirmPassword.Password; };
            this.tcMain.SelectionChanged += delegate
            {
                pbCurrentAuthorizationPassword.Password = string.Empty;
                pbAddUserPassword.Password = string.Empty;
                pbAddUserConfirmPassword.Password = string.Empty;
            };
        }

    }
}
