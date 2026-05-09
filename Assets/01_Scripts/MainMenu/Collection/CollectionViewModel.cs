using VContainer;

using RottenNoble.Core;
using RottenNoble.Core.UI;

namespace RottenNoble.MainMenu.Collection
{
    /// <summary>
    /// 도감 탭 ViewModel
    /// </summary>
    public class CollectionViewModel : ViewModelBase<CollectionView, CollectionModel>
    {
        SaveDataService _saveData;

        [Inject]
        void InjectServices(SaveDataService saveData) => _saveData = saveData;

        public override void Initialize(CollectionView view, CollectionModel model)
        {
            base.Initialize(view, model);

            // TODO: 도감 데이터 로드 및 UI 바인딩
        }

        public void SwitchTab(CollectionTab tab) => Model.CurrentTab.Value = tab;
    }
}
