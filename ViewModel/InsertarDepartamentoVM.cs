using ProjecteFinal.Base;
using ProjecteFinal.DAO;
using ProjecteFinal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjecteFinal.ViewModel
{
    public class InsertarDepartamentoVM : BaseViewModel
    {
        private readonly DepartamentoDAO departamentoDAO;
        public Departamento Departamento { get; set; }

        public InsertarDepartamentoVM()
        {
            departamentoDAO = new DepartamentoDAO();
            Departamento = new Departamento();
        }

        public async Task<bool> GuardarDepartamentoAsync()
        {
            try
            {
                if (Departamento == null)
                    throw new ArgumentException("El departamento no puede ser nulo.");

                if (string.IsNullOrWhiteSpace(Departamento.codigo) ||
                    string.IsNullOrWhiteSpace(Departamento.nombre) ||
                    string.IsNullOrWhiteSpace(Departamento.ubicacion))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Todos los campos son obligatorios.", "Aceptar");
                    return false;
                }

                // Verificar si el código ya está en uso antes de insertar
                var existente = await departamentoDAO.ObtenerDepartamentoPorCodigoAsync(Departamento.codigo);
                if (existente != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Aviso", "Ya existe un departamento con este código.", "Aceptar");
                    return false;
                }

                // Insertar nuevo departamento
                await departamentoDAO.GuardarDepartamentoAsync(Departamento);
                return true;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"No se pudo guardar el departamento: {ex.Message}", "Aceptar");
                return false;
            }
        }
    }
}