using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace DrawEngine.Renderer.Collections.Design
{
    public partial class ObjectChooseType : Form
    {
        private Type selectedType;
        private Type type;
        public ObjectChooseType(Type type)
        {
            this.InitializeComponent();
            this.type = type;
        }
        public Type SelectedType
        {
            get { return this.selectedType; }
            set { this.selectedType = value; }
        }
        private void ChooseObjectType_Load(object sender, EventArgs e)
        {
            Assembly[] assembliesLoaded = AppDomain.CurrentDomain.GetAssemblies();
            List<KeyValuePair<string, Type>> listType = new List<KeyValuePair<string, Type>>();
            foreach(Assembly loaded in assembliesLoaded){
                //Assembly ass = Assembly.GetAssembly(this.type);
                foreach(Type typeTemp in loaded.GetExportedTypes()){
                    if(!typeTemp.IsAbstract){
                        if(typeTemp.IsSubclassOf(this.type)){
                            listType.Add(new KeyValuePair<string, Type>(typeTemp.Name, typeTemp));
                        }
                    }
                }
            }
            //this.ddlObjectType.Items.Add(typeTemp);
            this.ddlObjectType.ValueMember = "Value";
            this.ddlObjectType.DisplayMember = "Key";
            this.ddlObjectType.DataSource = listType;
            this.ddlObjectType.SelectedIndex = 0;
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.selectedType = this.ddlObjectType.SelectedValue as Type;
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.selectedType = null;
        }
    }
}