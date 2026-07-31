using UnityEngine;

public static class AppSession
{
    private const string IsLoggedInKey = "IsLoggedIn";
    private const string UserIdKey = "UserId";
    private const string UsernameKey = "Username";
    private const string UserNameKey = "UserName";
    private const string UserSurnameKey = "UserSurname";
    private const string RoleIdKey = "RoleId";
    private const string RoleNameKey = "RoleName";

    public static bool HasActiveSession =>
        PlayerPrefs.GetInt(IsLoggedInKey, 0) == 1 &&
        PlayerPrefs.GetInt(UserIdKey, 0) > 0;

    public static int UserId =>
        PlayerPrefs.GetInt(UserIdKey, 0);

    public static string Username =>
        PlayerPrefs.GetString(UsernameKey, string.Empty);

    public static string Name =>
        PlayerPrefs.GetString(UserNameKey, string.Empty);

    public static string Surname =>
        PlayerPrefs.GetString(UserSurnameKey, string.Empty);

    public static int RoleId =>
        PlayerPrefs.GetInt(RoleIdKey, 0);

    public static string RoleName =>
        PlayerPrefs.GetString(RoleNameKey, string.Empty);

    public static void Save(
        int userId,
        string username,
        string name,
        string surname,
        int roleId,
        string roleName)
    {
        PlayerPrefs.SetInt(IsLoggedInKey, 1);
        PlayerPrefs.SetInt(UserIdKey, userId);

        PlayerPrefs.SetString(
            UsernameKey,
            username ?? string.Empty
        );

        PlayerPrefs.SetString(
            UserNameKey,
            name ?? string.Empty
        );

        PlayerPrefs.SetString(
            UserSurnameKey,
            surname ?? string.Empty
        );

        PlayerPrefs.SetInt(RoleIdKey, roleId);

        PlayerPrefs.SetString(
            RoleNameKey,
            roleName ?? string.Empty
        );

        PlayerPrefs.Save();
    }

    public static void Logout()
    {
        PlayerPrefs.DeleteKey(IsLoggedInKey);
        PlayerPrefs.DeleteKey(UserIdKey);
        PlayerPrefs.DeleteKey(UsernameKey);
        PlayerPrefs.DeleteKey(UserNameKey);
        PlayerPrefs.DeleteKey(UserSurnameKey);
        PlayerPrefs.DeleteKey(RoleIdKey);
        PlayerPrefs.DeleteKey(RoleNameKey);

        // Kullanýcý adý hatýrlama tercihi özellikle silinmez.
        // Böylece çýkýþ yaptýktan sonra giriþ ekranýnda kullanýcý adý
        // Beni Hatýrla seçiliyse tekrar gösterilebilir.

        PlayerPrefs.Save();
    }
}