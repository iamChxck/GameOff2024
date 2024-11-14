public class InputActionSingleton
{
    private static PlayerInputActions _instance;

    public static PlayerInputActions Instance
    {
        get
        {
            // Create the instance if it doesn't exist
            if (_instance == null)
            {
                _instance = new PlayerInputActions();
            }
            return _instance;
        }
    }
}