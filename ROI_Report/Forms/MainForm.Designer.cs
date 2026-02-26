namespace ROI_Report
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            pictureBoxImage = new PictureBox();
            groupBoxRoiList = new GroupBox();
            listBoxRoi = new ListBox();
            Load_Image_Btn = new Button();
            GrayImage = new Button();
            ColorImage = new Button();
            Save_Image_Btn = new Button();
            ROI_Rect_Btn = new Button();
            ROI_Circle_Btn = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBoxImage).BeginInit();
            groupBoxRoiList.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBoxImage
            // 
            pictureBoxImage.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxImage.Location = new Point(433, 68);
            pictureBoxImage.Name = "pictureBoxImage";
            pictureBoxImage.Size = new Size(820, 671);
            pictureBoxImage.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxImage.TabIndex = 0;
            pictureBoxImage.TabStop = false;
            // 
            // groupBoxRoiList
            // 
            groupBoxRoiList.Controls.Add(listBoxRoi);
            groupBoxRoiList.Location = new Point(12, 12);
            groupBoxRoiList.Name = "groupBoxRoiList";
            groupBoxRoiList.Size = new Size(415, 730);
            groupBoxRoiList.TabIndex = 1;
            groupBoxRoiList.TabStop = false;
            groupBoxRoiList.Text = "ROI 목록";
            // 
            // listBoxRoi
            // 
            listBoxRoi.Dock = DockStyle.Fill;
            listBoxRoi.FormattingEnabled = true;
            listBoxRoi.Location = new Point(3, 23);
            listBoxRoi.Name = "listBoxRoi";
            listBoxRoi.Size = new Size(409, 704);
            listBoxRoi.TabIndex = 0;
            listBoxRoi.SelectedIndexChanged += ListBoxRoi_SelectedIndexChanged;
            listBoxRoi.KeyDown += ListBoxRoi_KeyDown;
            listBoxRoi.MouseDown += ListBoxRoi_MouseDown;
            // 
            // Load_Image_Btn
            // 
            Load_Image_Btn.Image = (Image)resources.GetObject("Load_Image_Btn.Image");
            Load_Image_Btn.Location = new Point(433, 22);
            Load_Image_Btn.Name = "Load_Image_Btn";
            Load_Image_Btn.Size = new Size(40, 40);
            Load_Image_Btn.TabIndex = 2;
            Load_Image_Btn.UseVisualStyleBackColor = true;
            Load_Image_Btn.Click += Load_Image_Btn_Click;
            // 
            // GrayImage
            // 
            GrayImage.Image = (Image)resources.GetObject("GrayImage.Image");
            GrayImage.Location = new Point(691, 22);
            GrayImage.Name = "GrayImage";
            GrayImage.Size = new Size(40, 40);
            GrayImage.TabIndex = 2;
            GrayImage.UseVisualStyleBackColor = true;
            GrayImage.Click += GrayImage_Click;
            // 
            // ColorImage
            // 
            ColorImage.Image = (Image)resources.GetObject("ColorImage.Image");
            ColorImage.Location = new Point(645, 22);
            ColorImage.Name = "ColorImage";
            ColorImage.Size = new Size(40, 40);
            ColorImage.TabIndex = 2;
            ColorImage.UseVisualStyleBackColor = true;
            ColorImage.Click += ColorImage_Click;
            // 
            // Save_Image_Btn
            // 
            Save_Image_Btn.Image = (Image)resources.GetObject("Save_Image_Btn.Image");
            Save_Image_Btn.Location = new Point(479, 22);
            Save_Image_Btn.Name = "Save_Image_Btn";
            Save_Image_Btn.Size = new Size(40, 40);
            Save_Image_Btn.TabIndex = 3;
            Save_Image_Btn.UseVisualStyleBackColor = true;
            Save_Image_Btn.Click += Save_Image_Btn_Click;
            // 
            // ROI_Rect_Btn
            // 
            ROI_Rect_Btn.Image = (Image)resources.GetObject("ROI_Rect_Btn.Image");
            ROI_Rect_Btn.Location = new Point(851, 22);
            ROI_Rect_Btn.Name = "ROI_Rect_Btn";
            ROI_Rect_Btn.Size = new Size(40, 40);
            ROI_Rect_Btn.TabIndex = 4;
            ROI_Rect_Btn.UseVisualStyleBackColor = true;
            ROI_Rect_Btn.Click += ROI_Rect_Btn_Click;
            // 
            // ROI_Circle_Btn
            // 
            ROI_Circle_Btn.Image = (Image)resources.GetObject("ROI_Circle_Btn.Image");
            ROI_Circle_Btn.Location = new Point(897, 22);
            ROI_Circle_Btn.Name = "ROI_Circle_Btn";
            ROI_Circle_Btn.Size = new Size(40, 40);
            ROI_Circle_Btn.TabIndex = 4;
            ROI_Circle_Btn.UseVisualStyleBackColor = true;
            ROI_Circle_Btn.Click += ROI_Circle_Btn_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1262, 753);
            Controls.Add(ROI_Circle_Btn);
            Controls.Add(ROI_Rect_Btn);
            Controls.Add(Save_Image_Btn);
            Controls.Add(ColorImage);
            Controls.Add(GrayImage);
            Controls.Add(Load_Image_Btn);
            Controls.Add(groupBoxRoiList);
            Controls.Add(pictureBoxImage);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "MainForm";
            SizeGripStyle = SizeGripStyle.Hide;
            Text = "Favis ROI Inspection";
            ((System.ComponentModel.ISupportInitialize)pictureBoxImage).EndInit();
            groupBoxRoiList.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBoxImage;
        private GroupBox groupBoxRoiList;
        private ListBox listBoxRoi;
        private Button Load_Image_Btn;
        private Button GrayImage;
        private Button ColorImage;
        private Button Save_Image_Btn;
        private Button ROI_Rect_Btn;
        private Button ROI_Circle_Btn;
    }
}
