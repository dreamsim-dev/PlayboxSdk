namespace Playbox
{
    public static class InAppVerificationCongifuration
    {
        private static bool isSandbox = true;

        public static bool IsSandbox
        {
            get => isSandbox;
            set => isSandbox = value;
        }
    }
}