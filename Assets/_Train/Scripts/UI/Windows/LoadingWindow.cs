using _Train.Scripts.UI.Root;

namespace _Train.Scripts.UI.Windows
{
    public class LoadingWindow : Window
    {
        public static LoadingWindow Instance { get; private set; }

        private void Awake()
        {
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            Instance = this;
        }
    }
}
