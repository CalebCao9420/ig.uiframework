namespace IG.Module.UI{
    public interface IViewAnimation{
        IViewAnimation MoveIn();
        IViewAnimation MoveOut();
        System.Action  OnStartHandler   { get; set; }
        System.Action  OnCompleteHandler{ get; set; }
    }
}