namespace SuuchaStudio.Unity.Core.AdPlaying
{
    public class EmptyUserBehaviourRecord : IUserBehaviourRecord
    {
        private static IUserBehaviourRecord instance;
        public static IUserBehaviourRecord Instance
        {
            get
            {
                instance ??= new EmptyUserBehaviourRecord();
                return instance;
            }
        }
        public void AddBehaviour(string userId, string code, long value)
        {

        }
    }
}
