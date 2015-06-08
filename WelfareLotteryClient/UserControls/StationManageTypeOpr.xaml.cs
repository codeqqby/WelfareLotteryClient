using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using WelfareLotteryClient.DBModels;

namespace WelfareLotteryClient.UserControls
{
    /// <summary>
    /// StationManageTypeOpr.xaml 的交互逻辑
    /// </summary>
    public partial class StationManageTypeOpr : Window
    {
        readonly RoutedCommand _childCommandCheck=new RoutedCommand();
        Dictionary<string, ObservableCollection<AddStationManageTypeViewModel>> addedInfo = new Dictionary<string, ObservableCollection<AddStationManageTypeViewModel>>();
        // ObservableCollection<AddStationManageTypeViewModel> manage=new ObservableCollection<AddStationManageTypeViewModel>(); 
        readonly StringDictionary _keyAndValue=new StringDictionary();
        public StationManageTypeOpr()
        {
            InitializeComponent();

            CommandBinding binding = new CommandBinding {Command = _childCommandCheck};
            binding.CanExecute += Binding_CanExecute;
            binding.Executed += Binding_Executed;

            wpContentContainer.CommandBindings.Add(binding);

            ReloadAllTypes();
        }

        //重新加载所有类型
        private void ReloadAllTypes()
        {
            _keyAndValue.Clear();
            addedInfo.Clear();
            _allStationManageTypes.Clear();

            _allStationManageTypes = _operateStation.GetAllManageType();
            LoadAllTypes(_allStationManageTypes);
        }

        List<StationManageType> _allStationManageTypes=new List<StationManageType>(); 
        readonly OperateStationManageType _operateStation=new OperateStationManageType();
        private void btnCommitAll_Click(object sender, RoutedEventArgs e)
        {
            btnCommitAll.IsEnabled = false;
            foreach (var item in addedInfo)
            {
                //查出如不为null 说明原数据库中有
                StationManageType type = _allStationManageTypes.FirstOrDefault(p => p.Id.ToString() == item.Key);
                if (type==null)
                {
                    _allStationManageTypes.Add(new StationManageType {TypeName = _keyAndValue[item.Key],DetailsListSerialized = JsonConvert.SerializeObject(item.Value.Select(p=>p.ChildName))});
                }
                else
                {
                    type.TypeName =_keyAndValue[item.Key];
                    type.DetailsListSerialized = JsonConvert.SerializeObject(item.Value.Select(p => p.ChildName));
                }
            }

             _operateStation.AddManageType(_allStationManageTypes);
            
            ReloadAllTypes();
            MessageBox.Show("操作完成啦！");
            btnCommitAll.IsEnabled =true;
        }

        //删除本类
        private void BtnDelDetails_Click(object sender, RoutedEventArgs e)
        {
            Button btn=sender as Button;

            addedInfo.Remove(btn.Tag.ToString());
            _keyAndValue.Remove(btn.Tag.ToString());

            int id;
            bool parseResult= int.TryParse(btn.Tag.ToString(), out id);
            if (parseResult)
            {
                var ttt=_allStationManageTypes.Find(p=>p.Id== id);
                _allStationManageTypes.Remove(ttt);

                _operateStation.RemoveManageType(ttt);//在数据库中将其移除
            }

            StackPanel stack = VisualTreeHelper.GetParent(btn.Parent) as StackPanel;
            GroupBox box= stack.Parent as GroupBox;

            wpContentContainer.Children.Remove(box);
        }

        //删除子类
        private void BtnDelOperate(object sender, RoutedEventArgs e)
        {
            Button btn=sender as Button;

            if (btn!=null)
            {
                var collection = addedInfo[btn.Tag.ToString()];

                collection?.Remove(collection.FirstOrDefault(p => p.Identity == btn.Uid));
            }
        }

        //增加子分类
        private void Binding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Button btn = e.OriginalSource as Button;

            var temp = VisualTreeHelper.GetParent(btn);
            TextBox box = VisualTreeHelper.GetChild(temp, 0) as TextBox;

            if (string.IsNullOrEmpty(box.Text.Trim()))
            {
                return;
            }

            addedInfo[btn.Tag.ToString()].Add(new AddStationManageTypeViewModel {Identity = Guid.NewGuid().ToString(),ChildName = box.Text });
            box.Text = string.Empty;
        }

        private void Binding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            Button btn = e.OriginalSource as Button;
            var temp = VisualTreeHelper.GetParent(btn);
            TextBox box = VisualTreeHelper.GetChild(temp, 0) as TextBox;

            e.CanExecute = !string.IsNullOrEmpty(box.Text.Trim());

            e.Handled = true;
        }

        //增加销售站类型
        private void btnAddType_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtType.Text.Trim()))
            {
                MessageBox.Show("输入销售站类型！");
                return;
            }

            if (addedInfo.ContainsKey(txtType.Text.Trim()) || _operateStation.IfExistManageType(txtType.Text.Trim()))
            {
                MessageBox.Show("销售站类型已存在！");
                return;
            }
            string guid = Guid.NewGuid().ToString();
            _keyAndValue.Add(guid, txtType.Text.Trim());
            addedInfo.Add(guid, new ObservableCollection<AddStationManageTypeViewModel>());

            GroupBox box = CreateGroupBox(txtType.Text.Trim(), guid);
            //box.DataContext = addedInfo[txtType.Text.Trim()];

            wpContentContainer.Children.Add(box);
        }

        private void LoadAllTypes(List<StationManageType> all)
        {
            wpContentContainer.Children.Clear();

            foreach (var manageType in all)
            {
                var temp =
                    JsonConvert.DeserializeObject<List<string>>(
                        manageType.DetailsListSerialized);
                
                ObservableCollection<AddStationManageTypeViewModel> result = new ObservableCollection<AddStationManageTypeViewModel>();

                foreach (var type in temp)
                {
                    result.Add(new AddStationManageTypeViewModel { ChildName = type, Identity = Guid.NewGuid().ToString() });
                }
                string id = manageType.Id.ToString();
                _keyAndValue.Add(id, manageType.TypeName);
                addedInfo.Add(id, result);

                GroupBox box = CreateGroupBox(manageType.TypeName, id);

                wpContentContainer.Children.Add(box);

            }
        }

        //修改类型名称
        private void btnModifyType_Click(object sender, RoutedEventArgs e)
        {
            _keyAndValue[currentFocusedGroupBox]= txtType.Text;

            foreach (GroupBox box in wpContentContainer.Children.Cast<GroupBox>().Where(box => box.Tag.ToString() == currentFocusedGroupBox))
            {
                box.Header= txtType.Text;
                return;
            }
        }

        private string currentFocusedGroupBox;

        private void Box_GotFocus(object sender, RoutedEventArgs e)
        {
            GroupBox box = sender as GroupBox;

            txtType.Text = box.Header.ToString();

            currentFocusedGroupBox = box.Tag.ToString();
        }

        private GroupBox CreateGroupBox(string header,string guid)
        {
            GroupBox box = new GroupBox();
            box.GotFocus += Box_GotFocus;

            box.Header = header;
            box.Tag = guid;

           // box.SetBinding(HeaderedContentControl.HeaderProperty, new Binding("Parent"));

            StackPanel stackPanel = new StackPanel();
            WrapPanel wrapPanel = new WrapPanel();

            TextBox text = new TextBox
            {
                Width =150,
                //TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 5, 0),
                Style = this.FindResource("TextBoxCircleBorder") as Style
            };

            Button btn = new Button
            {
                Content = "增加详情",
                Tag = guid,
                Command = _childCommandCheck
            };

            Button btnDelDetails=new Button
            {
                Content = "删除本类",
                Tag = guid
            };
            btnDelDetails.Click += BtnDelDetails_Click;

            ListView view = new ListView {ItemsSource = addedInfo[guid],BorderThickness =new Thickness(0)};
            //view.SetBinding(ItemsControl.ItemsSourceProperty, new Binding("TypeChildren"));

            GridView gridView = new GridView();
            gridView.Columns.Add(new GridViewColumn {Width = 120,Header = "详情", DisplayMemberBinding = new Binding("ChildName")});

           
            var cell = new GridViewColumn {Width = 100, Header = "操作"};

            var temp = new DataTemplate();

            FrameworkElementFactory btnDel=new FrameworkElementFactory(typeof(Button),"btnDel");
            btnDel.SetValue(ContentProperty,"删除");
            btnDel.SetBinding(UidProperty, new Binding("Identity")); //知其属于哪个子类
            btnDel.SetValue(TagProperty, guid); //知其属于哪个父类
            btnDel.AddHandler(ButtonBase.ClickEvent,new RoutedEventHandler(BtnDelOperate));

            temp.VisualTree = btnDel;
            cell.CellTemplate= temp;
            gridView.Columns.Add(cell);

            view.View = gridView;

            wrapPanel.Children.Add(text);
            wrapPanel.Children.Add(btn);
            wrapPanel.Children.Add(btnDelDetails);

            stackPanel.Children.Add(wrapPanel);
            stackPanel.Children.Add(view);

            box.Content = stackPanel;

            return box;
        }
    }

    //AddTypeTemplate

    public class AddStationManageTypeViewModel
    {
        public string Identity { get; set; }
        public string ChildName { get; set; }
    }
}
