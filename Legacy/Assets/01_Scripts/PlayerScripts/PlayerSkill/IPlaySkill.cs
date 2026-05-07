/*
 * 생성자
 * 스킬 발동
 * 스킬 준비 (인디케이터)
 * 스킬 실행
 */



public interface IPlaySkill
{
    public void Init(Player player);
    public void SkillActivated();

    public void SkillReady();

    public void SkillExecute();
    public void SkillDone();

    public void SkillUpdate(float deltaTime);
}