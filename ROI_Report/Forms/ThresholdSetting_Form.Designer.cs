namespace ROI_Report.Forms
{
    partial class ThresholdSetting_Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Threshold_Slider = new TrackBar();
            ThresholdValue = new Label();
            label2 = new Label();
            pictureBoxPreview = new PictureBox();
            btnApply = new Button();
            btnCancel = new Button();
            ((System.ComponentModel.ISupportInitialize)Threshold_Slider).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPreview).BeginInit();
            SuspendLayout();
            // 
            // Threshold_Slider
            // 
            Threshold_Slider.Location = new Point(21, 50);
            Threshold_Slider.Maximum = 255;
            Threshold_Slider.Name = "Threshold_Slider";
            Threshold_Slider.Size = new Size(279, 56);
            Threshold_Slider.TabIndex = 0;
            Threshold_Slider.Value = 127;
            // 
            // ThresholdValue
            // 
            ThresholdValue.Font = new Font("맑은 고딕", 13.8F);
            ThresholdValue.Location = new Point(306, 50);
            ThresholdValue.Name = "ThresholdValue";
            ThresholdValue.Size = new Size(95, 42);
            ThresholdValue.TabIndex = 1;
            ThresholdValue.Text = "127";
            ThresholdValue.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.Font = new Font("맑은 고딕", 13.8F);
            label2.Location = new Point(12, 9);
            label2.Name = "label2";
            label2.Size = new Size(134, 36);
            label2.TabIndex = 2;
            label2.Text = "Threshold";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pictureBoxPreview
            // 
            pictureBoxPreview.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxPreview.Location = new Point(21, 110);
            pictureBoxPreview.Name = "pictureBoxPreview";
            pictureBoxPreview.Size = new Size(380, 250);
            pictureBoxPreview.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxPreview.TabIndex = 3;
            pictureBoxPreview.TabStop = false;
            // 
            // btnApply
            // 
            btnApply.Location = new Point(226, 375);
            btnApply.Name = "btnApply";
            btnApply.Size = new Size(85, 35);
            btnApply.TabIndex = 4;
            btnApply.Text = "적용";
            btnApply.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(316, 375);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(85, 35);
            btnCancel.TabIndex = 5;
            btnCancel.Text = "취소";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // ThresholdSetting_Form
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(420, 420);
            Controls.Add(btnCancel);
            Controls.Add(btnApply);
            Controls.Add(pictureBoxPreview);
            Controls.Add(label2);
            Controls.Add(ThresholdValue);
            Controls.Add(Threshold_Slider);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ThresholdSetting_Form";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Threshold 설정";
            ((System.ComponentModel.ISupportInitialize)Threshold_Slider).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPreview).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TrackBar Threshold_Slider;
        private Label ThresholdValue;
        private Label label2;
        private PictureBox pictureBoxPreview;
        private Button btnApply;
        private Button btnCancel;
    }
}