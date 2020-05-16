using static BaseMonoBehaviour;

public interface IModify
{
    RenderLayer GetLayer();
    Defaults GetDefaults();
    void SetScale(float multiplier);
}