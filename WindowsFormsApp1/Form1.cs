using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    using Classes;
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static int printField(Type type1, List<Control> controls, int x, int y)
        {
            FieldInfo[] fieldinfo1 = type1.GetFields();
            foreach (var field1 in fieldinfo1)
            {
                Type type2 = Type.GetType(field1.FieldType.ToString());
                if (type2.IsEnum)
                {
                    controls.Add(new Label() { Font = new Font("Microsoft Sans Serif", 12), Size = new Size(300, 20), Location = new Point(0 + x, 10 + y * 28), Text = type1.Name + "   " + field1.Name });
                    y++;
                    ComboBox buf = new ComboBox() { Font = new Font("Microsoft Sans Serif", 12), Size = new Size(300, 28), DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList, Name = field1.Name, Location = new Point(0 + x, 10 + y * 28) };
                    FieldInfo[] fieldinfo2 = type2.GetFields(BindingFlags.Public | BindingFlags.Static);
                    foreach (var field2 in fieldinfo2)
                    {
                        buf.Items.Add(field2.Name.ToString());
                    }
                    controls.Add(buf);
                    y++;
                }
                else if ((type2.IsClass) && (type2.Name != "String"))
                {
                    y = printField(field1.FieldType, controls, x, y);
                }
                else
                {
                    controls.Add(new Label() { Font = new Font("Microsoft Sans Serif", 12), Size = new Size(300, 20), Location = new Point(0 + x, 10 + y * 28), Text = type1.Name + "   " + field1.Name });
                    y++;
                    controls.Add(new TextBox() { Font = new Font("Microsoft Sans Serif", 12), Size = new Size(300, 28), Name = field1.Name, Location = new Point(0 + x, 10 + y * 28) });
                    y++;
                }
               
            }
            return y;
        }
        public static int printVal(Object obj,List<Control> controls, int x, int y)
        {
            if (obj != null)
            {
                Type type1 = obj.GetType();
                Console.WriteLine(type1.Name);
                FieldInfo[] fieldinfo1 = type1.GetFields();
                foreach (var field1 in fieldinfo1)
                {
                    Type type2 = Type.GetType(field1.FieldType.ToString());
                    if (type2.IsEnum)
                    {
                        controls.Add(new Label() { Font = new Font("Microsoft Sans Serif", 12), Size = new Size(300, 20), Location = new Point(0 + x, 28 + y * 30), Text = type1.Name + "   " + field1.Name });
                        y++;
                        ComboBox buf = new ComboBox() { Font = new Font("Microsoft Sans Serif", 12), Size = new Size(300, 28), DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList, Name = field1.Name, Location = new Point(0 + x, 28 + y * 30) };
                        FieldInfo[] fieldinfo2 = type2.GetFields(BindingFlags.Public | BindingFlags.Static);
                        foreach (var field2 in fieldinfo2)
                        {
                            buf.Items.Add(field2.Name.ToString());
                        }
                        buf.Text = field1.GetValue(obj).ToString();
                        controls.Add(buf);
                        y++;
                    }
                    else if ((type2.IsClass) && (type2.Name != "String"))
                    {
                        y = printVal(field1.GetValue(obj), controls, x, y);
                    }
                    else
                    {
                        controls.Add(new Label() { Font = new Font("Microsoft Sans Serif", 12), Size = new Size(300, 20), Location = new Point(0 + x, 28 + y * 30), Text = type1.Name + "   " + field1.Name });
                        y++;
                        controls.Add(new TextBox() { Text = field1.GetValue(obj).ToString(), Font = new Font("Microsoft Sans Serif", 12), Size = new Size(300, 28), Name = field1.Name, Location = new Point(0 + x, 28 + y * 30) });
                        y++;
                    }
                }
            }
            return y;
        }
        public static bool setVal(Object obj,List<Control> controls, List<Object> objects)
        {
            if (obj != null)
            {
                Type type1 = obj.GetType();
                objects.Add(obj);
                FieldInfo[] fieldinfo1 = type1.GetFields();
                foreach (var field1 in fieldinfo1)
                {
                    Type type2 = Type.GetType(field1.FieldType.ToString());
                    if ((type2.IsClass) && (type2.Name != "String"))
                    {
                        setVal(field1.GetValue(obj), controls, objects);
                    }
                    else if (type2.IsEnum)
                    {
                        FieldInfo[] fieldinfo2 = type2.GetFields(BindingFlags.Public | BindingFlags.Static);
                        bool flag = false;
                        foreach (Control control in controls)
                        {
                            if (control.Name.ToString() == field1.Name.ToString())
                            {
                                Object val = control.Text;
                                foreach (var field2 in fieldinfo2)
                                {
                                    if (val.ToString() == field2.Name)
                                    {
                                        field1.SetValue(obj, field2.GetValue(obj));
                                    }
                                }
                            }
                        }
                        if (flag)
                        {
                            MessageBox.Show("Неправильно заполнено поле " + field1.Name);
                            return false;
                        }
                    }
                    else
                    {
                        Object val = 0;
                        foreach (Control control in controls)
                        {
                            if (control.Name.ToString() == field1.Name.ToString())
                            {
                                val = control.Text;
                            }
                        }
                        try
                        {
                            val = Convert.ChangeType(val, field1.FieldType);
                        }
                        catch
                        {
                            MessageBox.Show("Неправильно заполнено поле " + field1.Name);
                            return false;
                        }
                        field1.SetValue(obj, val);
                    }
                }
            }
            return true;
        }
        public static Object create_obj(Type loc_type)
        {
            ConstructorInfo[] cons = loc_type.GetConstructors();
            ParameterInfo[] pars = cons[0].GetParameters();
            List<Object> test = new List<Object>();
            if (pars.Length == 0)
            {
                return Activator.CreateInstance(loc_type);
            }
            else
            {
                foreach (var para in pars)
                {
                    test.Add(create_obj(para.ParameterType));
                }
            }
            return Activator.CreateInstance(loc_type, test.ToArray());
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Type[] typelist = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace == "Classes").ToArray();
            foreach (Type type in typelist)
            {
               
                if (type.IsClass)
                {
                    comboBox1.Items.Add(type.Name);
                }
            }
        }
        List<Control> controls = new List<Control>();
        Object bufObject;
        List<Object> allObj = new List<Object>();

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (Control control in controls)
            {
                this.Controls.Remove(control);
            }
            controls.Clear();
            bufObject = create_obj(Type.GetType("Classes."+comboBox1.Text));
            printField(bufObject.GetType(), controls, 482, 0);
            foreach (Control control in controls)
            {
                this.Controls.Add(control);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<Object> loc_arrObj = new List<Object>();
            if (setVal(bufObject, controls, loc_arrObj))
            {
                foreach (Control control in controls)
                {
                    this.Controls.Remove(control);
                }
                controls.Clear();
                int count = allObj.Count();
                foreach (Object obj in loc_arrObj)
                {
                    allObj.Add(obj);
                    listBox1.Items.Add(obj.ToString()+"_"+count.ToString());
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (Control control in controls)
            {
                this.Controls.Remove(control);
            }
            controls.Clear();
            printVal(allObj[listBox1.SelectedIndex], controls, 482, 0);
            foreach (Control control in controls)
            {
                this.Controls.Add(control);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            List<Object> loc_arrObj = new List<Object>();
            if (setVal(allObj[listBox1.SelectedIndex], controls, loc_arrObj))
            {
                foreach (Control control in controls)
                {
                    this.Controls.Remove(control);
                }
                controls.Clear();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            allObj.RemoveAt(listBox1.SelectedIndex);
            listBox1.Items.RemoveAt(listBox1.SelectedIndex);
        }
    }
}
namespace Classes
{
    enum yes_no
    {
        Yes,
        No,
    }
    class airplane : transport
    {
       // public int flight_height;
        public int wingsspan;
        //  public int wing_area;
        public yes_no auto_pilot;
        public engine engine;
        public airplane(engine eng)
        {
            engine = eng;
        }
    }
    class auto_transport : transport
    {
        public enum drive
        {
            Front_wheel_drive,
            Real_wheel_drive,
            Four_wheel_drive,
        }
        // public int number_of_wheels;
        //public int tire_diameter;
        public int tire_width;
        //  public int tire_pressure;
        public drive drive_unit;
    }
    class carpark
    {
        public int area;
        //   public int car_capacity;
        //    public int number_of_cars;
        //    public int number_of_workers;
        public int profit;
        public yes_no car_wash;
        public yes_no gas_station;
        public yes_no service_station;
        public ice_auto auto;
        public employee employee;
        public carpark(employee emp, ice_auto aut)
        {
            auto = aut;
            employee = emp;
        }
    }
    class electro_auto : auto_transport
    {
        enum battery
        {

            Lithium_ion,
            Aluminum_ion,
            Lithium_sulfur,

        }
        //  public int battery_capacity;
        public int charging_time;
        //  public int power_reserve;
        public int battery_consumption;
        public yes_no quick_charge;
        public engine engine;
        public electro_auto(engine eng)
        {
            engine = eng;
        }
    }
    class employee
    {
        public enum Gender
        {
            Female,
            Male,
            Agender,
            Bigender,
        }
        public int height;
        //    public int weight;
        public int age;
        //     public string education;
        //     public string name;
        public string surname;
        public engine engine;
        public Gender gender;
        public int salary;
    }
    public class engine
    {
        public int weight;
        public int year_of_creation;
        public int service_life;
        public int power;
        public int torque;
    }
    abstract class transport
    {
        //   public int length;
        public int weight;
        public int height;
        // public int width;
        //    public int carrying;
        public string color;
    }
    class ice_auto : auto_transport
    {
        public enum fuel
        {
            Gas,
            Petrol,
            Diesel,
        }
        // public int gas_tank_volume;
        // public int fuel_consumption;
        public int oil_consumption;
        // public int co2_emissions;
        public fuel fuel_grade;
        public engine engine;
        public ice_auto(engine eng)
        {
            engine = eng;
        }
    }
}

