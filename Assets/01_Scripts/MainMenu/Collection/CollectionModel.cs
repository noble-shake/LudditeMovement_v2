using R3;

using RottenNoble.Core.UI;

namespace RottenNoble.MainMenu.Collection
{
    /// <summary>
    /// 도감 탭 데이터 모델
    /// </summary>
    public class CollectionModel : ModelBase
    {
        public ReactiveProperty<CollectionTab> CurrentTab { get; } = new(CollectionTab.Monster);
    }

    public enum CollectionTab
    {
        Monster,    // 몬스터 로어
        HeroStory,  // 영웅 스토리 조각
        Affinity,   // 영웅 간 친밀도
    }
}
