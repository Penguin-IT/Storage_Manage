using StorageManage.Models;
using StorageManage.ViewModels;
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
            set { _searchText = value; OnPropertyChanged("SearchText"); }
        }

        // Danh sách kết quả tìm kiếm (Ví dụ là Sản phẩm)
        private ObservableCollection<SanPham> _searchResult;
        public ObservableCollection<SanPham> SearchResult
        {
            get => _searchResult;
            set { _searchResult = value; OnPropertyChanged("SearchResult"); }
        }

        public ICommand SearchCommand { get; set; }

        public SearchViewModel()
        {
            SearchResult = new ObservableCollection<SanPham>();

            
            SearchCommand = new RelayCommand((p) => ExecuteSearch());
        }

        void ExecuteSearch()
        {
   
            using (var db = new QLKEntities())
            {

                if (string.IsNullOrEmpty(SearchText))
                {
                    var all = db.SanPhams.ToList();
                    SearchResult = new ObservableCollection<SanPham>(all);
                    return;
                }
                var res = db.SanPhams.Where(x => x.MaSP.Contains(SearchText) ||
                                         x.TenSP.Contains(SearchText)).ToList();

                SearchResult = new ObservableCollection<SanPham>(res);
            }
        }
    }
}
