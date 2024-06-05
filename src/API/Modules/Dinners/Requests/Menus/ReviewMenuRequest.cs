namespace API.Modules.Dinners.Requests.Menus;

public sealed record ReviewMenuRequest(decimal Rate,
    string Comment);
