using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LKS_JATIM_2024
{
    public partial class Form1 : Form
    {
        CurrencyConverterEntities ccEntity = new CurrencyConverterEntities();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            currencyBindingSource.DataSource = ccEntity.Currency.ToList();
            periodBindingSource.DataSource = ccEntity.Period.ToList();
            ConvertBinding.DataSource = ccEntity.Currency.ToList();

            comboBox2.DisplayMember = "abbreviation";
            comboBox2.ValueMember = "id";
            comboBox2.DataSource = ConvertBinding.DataSource;

            comboBox1.SelectedValue = 0;
            comboBox2.SelectedValue = 0;
            comboBoxPeriod.SelectedValue = 0;
            label3.Text = string.Empty;
            label4.Text = string.Empty;

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idSelect = Convert.ToInt32(comboBox1.SelectedValue);
            var nama_mata_uang = ccEntity.Currency.FirstOrDefault(cc => cc.id == idSelect);
            label3.Text = nama_mata_uang?.name ?? "Invalid Name Currency";
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idSelect = Convert.ToInt32(comboBox2.SelectedValue);
            var nama_mata_uang = ccEntity.Currency.FirstOrDefault(cc => cc.id == idSelect);
            label4.Text = nama_mata_uang?.name ?? "Invalid Name Currency";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var asal_mata_uang = comboBox1.SelectedValue;
            comboBox1.SelectedValue = comboBox2.SelectedValue;
            comboBox2.SelectedValue = asal_mata_uang;
        }

        // 1. Kita Ambil dulu Value dari masing masing combo box
        private decimal ambil_nilai(decimal amount) {
            if (comboBox1.SelectedValue == null || comboBox1.SelectedValue == null) return 0;
            {
                var asal_mata_uang = Convert.ToInt32(comboBox1.SelectedValue);
                var tujuan_mata_uang = Convert.ToInt32(comboBox2.SelectedValue);
                var period = Convert.ToInt32(comboBoxPeriod.SelectedValue);
                if (asal_mata_uang == tujuan_mata_uang)
                {
                    MessageBox.Show("Abbreviation Must be Different!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                if (period == 0)
                {
                    MessageBox.Show("Select Period first!", "Warnig!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                decimal rate = kalkulasi_kurs(asal_mata_uang, tujuan_mata_uang, period);

                return amount * rate;
            }
        }

        // 2. Buat fungsi Untuk menghitung kurs mata uang ke dolar
        private decimal kalkulasi_kurs(int mata_uang_asal, int mata_uang_tujuan, int period){
            var kurs_mata_uang_asal = ccEntity.USDExchangeRate.Where(i => i.currency_id == mata_uang_asal && i.period_id == period).FirstOrDefault();
            var kurs_mata_uang_tujuan = ccEntity.USDExchangeRate.Where(i => i.currency_id == mata_uang_tujuan && i.period_id == period).FirstOrDefault();
            if (kurs_mata_uang_asal == null || kurs_mata_uang_tujuan == null)
            {
                return 0;
            }
            var hasil_kurs = kurs_mata_uang_asal.rate / kurs_mata_uang_tujuan.rate;
            return hasil_kurs;
        }

        // 3. ambil inputan pengguna di textbox
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(textBox1.Text, out decimal input))
            {
                decimal hasil_akhir = ambil_nilai(input);
                textBox2.Text = hasil_akhir.ToString("N2");
            }
            else
            {
                MessageBox.Show("Input Invalid!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox2.Clear();
            }
        }


    }
}
