using System;
using System.ComponentModel.DataAnnotations;

namespace CodeQuestBackend.Models.Dtos;

public class CreateUserDto
{
    [Required(ErrorMessage = "El campo username es requerido")]
    public string? Username { get; set; }
    [Required(ErrorMessage = "El campo password es requerido")]
    public string? Password { get; set; }
    [Required(ErrorMessage = "El campo email es requerido")]

    public string? Email { get; set; }
    [Required(ErrorMessage = "El campo role es requerido")]

    public string? Role { get; set; }

}