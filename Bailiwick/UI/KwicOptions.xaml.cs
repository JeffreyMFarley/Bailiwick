using Esoteric.UI;
using System.Windows.Controls;

namespace Bailiwick.UI
{
    /// <summary>
    /// Interaction logic for KwicOptions.xaml
    /// </summary>
    public partial class KwicOptions : ToolBar, IView
    {
        public KwicOptions()
        {
            InitializeComponent();
        }
        
        #region IView Members

        /// <summary>
        /// Provides the data context for this view
        /// </summary>
        public IViewModel Model
        {
            get
            {
                return this.DataContext as IViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        #endregion
	
    }
}
