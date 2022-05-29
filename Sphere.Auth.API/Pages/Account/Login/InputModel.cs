// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using System.ComponentModel.DataAnnotations;

namespace Sphere.Auth.API.Pages.Login;

public class InputModel
{
    [Required(AllowEmptyStrings = false)]
    public string Username { get; set; } = string.Empty;
        
    [Required(AllowEmptyStrings = false)]
    public string Password { get; set; } = string.Empty;
        
    public bool RememberLogin { get; set; }

    public string ReturnUrl { get; set; } = string.Empty;

    public string Button { get; set; } = string.Empty;
}
