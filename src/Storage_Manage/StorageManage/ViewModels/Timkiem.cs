using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Storage_Manage.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); }
        }

        // Danh sách kết quả tìm kiếm (Ví dụ là Sản phẩm)
        private ObservableCollection<object> _searchResult;
        public ObservableCollection<object> SearchResult
        {
            get => _searchResult;
            set { _searchResult = value; OnPropertyChanged(); }
        }

        public ICommand SearchCommand { get; set; }

        public SearchViewModel()
        {
            SearchResult = new ObservableCollection<object>();
           
        }
    }
}
