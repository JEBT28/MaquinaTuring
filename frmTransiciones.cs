﻿using System;
using System.Windows.Forms;

namespace MT
{
    public enum Operaciones { Borrar, Escribir }
    public enum Movimientos { Nada, Izquierda, Derecha }
    public partial class frmTransiciones : Form
    {
        public Maquina maquina;
        public Transicion transicionSeleccionada;
        public frmTransiciones(Maquina m)
        {
            this.maquina = m;
            InitializeComponent();
        }

        private void frmTransiciones_Load(object sender, EventArgs e)
        {
            //Metodo que carga los componentes
            cmbOperacion.DataSource = Enum.GetValues(typeof(Operaciones));
            cmbMovimiento.DataSource = Enum.GetValues(typeof(Movimientos));

            Array.ForEach(maquina.Alfabeto, (s) => { cmbSimboloLeido.Items.Add(s); cmbSimboloNuevo.Items.Add(s); });
            cmbSimboloLeido.Items.Add(maquina.EspacioBlanco);
            cmbSimboloLeido.Items.Add(maquina.Marca);
            cmbSimboloNuevo.Items.Add(maquina.EspacioBlanco);
            cmbSimboloNuevo.Items.Add(maquina.Marca);

            maquina.Estados.ForEach(est =>
            {

                if (est.Etiqueta != maquina.EstadoFinal)
                {
                    cmbEstado.Items.Add(est.Etiqueta);
                }
                cmbEstadoSiguiente.Items.Add(est.Etiqueta);

            });

            dgTransiciones.Columns.Add("Estado Actual", "Estado Actual");
            dgTransiciones.Columns.Add("Simbolo Leido", "Simbolo Leido");
            dgTransiciones.Columns.Add("Operacion", "Operacion");
            dgTransiciones.Columns.Add("Simbolo Escrito", "Simbolo Escrito");
            dgTransiciones.Columns.Add("Nuevo Estado", "Nuevo Estado");
            dgTransiciones.Columns.Add("Movimiento", "Movimiento");
            dgTransiciones.ReadOnly = true;
            dgTransiciones.AllowUserToAddRows = false;
            dgTransiciones.AllowUserToDeleteRows = false;
            dgTransiciones.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            MostrarTransiciones();
        }

        private void btnAgregarTransicion_Click(object sender, EventArgs e)
        {
            //Metodo que agrega una transicion
            if (cmbEstado.SelectedItem != null && cmbEstadoSiguiente.SelectedItem != null && //Validacion de que los Combobox tengan informacion
                cmbMovimiento.SelectedItem != null && cmbOperacion.SelectedItem != null &&
                cmbSimboloLeido.SelectedItem != null && cmbSimboloNuevo.SelectedItem != null
                )
            {
                Transicion transicion = new Transicion() //Se cra la transicion
                {
                    q = cmbEstado.SelectedItem.ToString(),
                    ValorBuscado = char.Parse(cmbSimboloLeido.SelectedItem.ToString()),
                    Operacion = (Operaciones)cmbOperacion.SelectedItem,
                    Movimiento = (Movimientos)cmbMovimiento.SelectedItem,
                    ValorNuevo = char.Parse(cmbSimboloNuevo.SelectedItem.ToString()),
                    p = cmbEstadoSiguiente.SelectedItem.ToString()
                };
                //Evalua si la transicion ya existe  en la lista
                if (maquina.ListaTransiciones.Contains(transicion))
                {
                    MessageBox.Show("Informacion", "Ya existe una configuracion para el estado  y simbolo leido elegidos");
                    return;
                }
                else
                {
                    maquina.AgregarTransicion(transicion);
                }
            }
            MostrarTransiciones();

        }

        void MostrarTransiciones()
        {
            //Metodo que muestra las transiciones en la tabla
            dgTransiciones.Rows.Clear();
            maquina.ListaTransiciones.ForEach(t => dgTransiciones.Rows.Add(t.q, t.ValorBuscado, t.Operacion, t.ValorNuevo, t.p, t.Movimiento));

        }

        private void btnEditarTransicion_Click(object sender, EventArgs e)
        {
            //Metodo que edita una transicion
            if (cmbEstado.SelectedItem != null && cmbEstadoSiguiente.SelectedItem != null &&
               cmbMovimiento.SelectedItem != null && cmbOperacion.SelectedItem != null &&
               cmbSimboloLeido.SelectedItem != null && cmbSimboloNuevo.SelectedItem != null
               )
            {
                Transicion transicion = new Transicion()
                {
                    q = cmbEstado.SelectedItem.ToString(),
                    ValorBuscado = char.Parse(cmbSimboloLeido.SelectedItem.ToString()),
                    Operacion = (Operaciones)cmbOperacion.SelectedItem,
                    Movimiento = (Movimientos)cmbMovimiento.SelectedItem,
                    ValorNuevo = char.Parse(cmbSimboloNuevo.SelectedItem.ToString()),
                    p = cmbEstadoSiguiente.SelectedItem.ToString()
                };

                string msg = maquina.EditarTransicion(transicionSeleccionada, transicion);
                if (msg.Equals("Ok"))
                {
                    MessageBox.Show(msg,"Informacion" );
                }
            }
            else
            {
                MessageBox.Show("Valide que esta insertando los datos requeridos");
            }
            MostrarTransiciones();

        }

        private void btnEliminarTransicion_Click(object sender, EventArgs e)
        {
            //Metodo que elimina la transicion
            if (cmbEstado.SelectedItem != null && cmbEstadoSiguiente.SelectedItem != null &&
               cmbMovimiento.SelectedItem != null && cmbOperacion.SelectedItem != null &&
               cmbSimboloLeido.SelectedItem != null && cmbSimboloNuevo.SelectedItem != null
               )
            {
                string msg = maquina.EliminarTransicion(transicionSeleccionada);
                if (msg.Equals("Ok"))
                {
                    MessageBox.Show(msg,"Informacion");
                }
            }
            else
            {
                MessageBox.Show("Valide que esta insertando los datos requeridos");
            }
            MostrarTransiciones();
        }

        private void dgTransiciones_SelectionChanged(object sender, EventArgs e)
        {
            //Evento que detecta cuando el usuario selecciona una transicion de la tabla
            if (dgTransiciones.SelectedRows.Count == 0)
            {
                return;
            }
            else
            {

                DataGridViewRow dr = dgTransiciones.SelectedRows[0];

                transicionSeleccionada = new Transicion()
                {
                    q = dr.Cells["Estado Actual"].Value.ToString(),
                    ValorBuscado = (char)dr.Cells["Simbolo Leido"].Value,
                    Operacion = (Operaciones)dr.Cells["Operacion"].Value,
                    ValorNuevo = (char)dr.Cells["Simbolo Escrito"].Value,
                    p = dr.Cells["Nuevo Estado"].Value.ToString(),
                    Movimiento = (Movimientos)dr.Cells["Movimiento"].Value
                };

                cmbEstado.SelectedItem = transicionSeleccionada.q;
                cmbEstadoSiguiente.SelectedItem = transicionSeleccionada.p;
                cmbMovimiento.SelectedItem = transicionSeleccionada.Movimiento;
                cmbOperacion.SelectedItem = transicionSeleccionada.Operacion;
                cmbSimboloLeido.SelectedItem = transicionSeleccionada.ValorBuscado;
                cmbSimboloNuevo.SelectedItem = transicionSeleccionada.ValorNuevo;
            }
        }
    }
}
