using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FileCatalog
{
	public class Form1 : Form
	{
		private IContainer components = null;

		private Label label1;

		private Label label2;

		private NumericUpDown num1;

		public Form1()
		{
			this.InitializeComponent();
		}

		private void Form1_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.All;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}

		private void Form1_DragDrop(object sender, DragEventArgs e)
		{
			string[] array = (string[])e.Data.GetData(DataFormats.FileDrop, false);
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				string text2 = text;
				if (File.Exists(text2))
				{
					FileInfo fileInfo = new FileInfo(text2);
					string a = fileInfo.FullName.Substring(fileInfo.FullName.LastIndexOf(".")).ToLower();
					if (a == ".jpg" || a == ".jpeg" || a == ".png")
					{
						int num = (int)this.num1.Value;
						if (num > 100)
						{
							num = 100;
							this.num1.Value = 100m;
						}
						if (num < 0)
						{
							num = 1;
							this.num1.Value = 1m;
						}
						this.GetCompressImage(fileInfo.FullName, fileInfo.FullName.Substring(0, fileInfo.FullName.LastIndexOf("\\")) + "\\zip_" + fileInfo.Name, num);
					}
				}
				else if (Directory.Exists(text2))
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(text2);
					string path = directoryInfo.FullName + "_zip";
					Directory.CreateDirectory(path);
					FileInfo[] files = directoryInfo.GetFiles("*");
					for (int j = 0; j < files.Length; j++)
					{
						FileInfo fileInfo = files[j];
						string a = fileInfo.FullName.Substring(fileInfo.FullName.LastIndexOf(".")).ToLower();
						if (a == ".jpg" || a == ".jpeg" || a == ".png")
						{
							int num = (int)this.num1.Value;
							if (num > 100)
							{
								num = 100;
								this.num1.Value = 100m;
							}
							if (num < 0)
							{
								num = 1;
								this.num1.Value = 1m;
							}
							this.GetCompressImage(fileInfo.FullName, fileInfo.FullName.Substring(0, fileInfo.FullName.LastIndexOf("\\")) + "_zip\\" + fileInfo.FullName.Substring(fileInfo.FullName.LastIndexOf("\\")), num);
						}
						else
						{
							fileInfo.CopyTo(fileInfo.FullName.Substring(0, fileInfo.FullName.LastIndexOf("\\")) + "_zip\\" + fileInfo.FullName.Substring(fileInfo.FullName.LastIndexOf("\\")), false);
						}
					}
				}
			}
		}

		public static bool GetPicThumbnail(string sFile, string outPath, int num = 100)
		{
			Image image = Image.FromFile(sFile);
			ImageFormat rawFormat = image.RawFormat;
			EncoderParameters encoderParameters = new EncoderParameters();
			long[] value = new long[]
			{
				(long)num
			};
			EncoderParameter encoderParameter = new EncoderParameter(Encoder.Quality, value);
			encoderParameters.Param[0] = encoderParameter;
			bool result;
			try
			{
				ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
				ImageCodecInfo imageCodecInfo = null;
				for (int i = 0; i < imageEncoders.Length; i++)
				{
					if (imageEncoders[i].FormatDescription.ToLower() == "jpeg")
					{
						imageCodecInfo = imageEncoders[i];
						break;
					}
				}
				if (imageCodecInfo != null)
				{
					image.Save(outPath, imageCodecInfo, encoderParameters);
				}
				else
				{
					image.Save(outPath, rawFormat);
				}
				result = true;
			}
			catch
			{
				result = false;
			}
			finally
			{
				image.Dispose();
				image.Dispose();
			}
			return result;
		}

		public string GetMimeType(Image image)
		{
			ImageCodecInfo[] imageDecoders = ImageCodecInfo.GetImageDecoders();
			ImageCodecInfo[] array = imageDecoders;
			string result;
			for (int i = 0; i < array.Length; i++)
			{
				ImageCodecInfo imageCodecInfo = array[i];
				if (imageCodecInfo.FormatID == image.RawFormat.Guid)
				{
					result = imageCodecInfo.MimeType;
					return result;
				}
			}
			result = "image/unknown";
			return result;
		}

		public bool GetCompressImage(string srcPath, string destPath, int quality)
		{
			bool result = false;
			Image image = null;
			Image image2 = null;
			Graphics graphics = null;
			try
			{
				image = Image.FromFile(srcPath, false);
				string mimeType = this.GetMimeType(image);
				FileInfo fileInfo = new FileInfo(srcPath);
				int num = image.Width;
				int num2 = image.Height;
				float num3 = (float)image.Height / (float)image.Width;
				float num4 = (float)num2 / (float)num;
				if (num4 > num3)
				{
					num2 = Convert.ToInt32((float)num * num3);
				}
				else if (num4 < num3)
				{
					num = Convert.ToInt32((float)num2 / num3);
				}
				image2 = new Bitmap(num, num2);
				graphics = this.GetGraphics(image2);
				graphics.DrawImage(image, new Rectangle(0, 0, num, num2), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
				if (destPath == srcPath)
				{
					image.Dispose();
				}
				this.SaveImage2File(destPath, image2, quality, mimeType);
				result = true;
			}
			catch (Exception var_10_EE)
			{
			}
			finally
			{
				if (image != null)
				{
					image.Dispose();
				}
				if (image2 != null)
				{
					image2.Dispose();
				}
				if (graphics != null)
				{
					graphics.Dispose();
				}
			}
			return result;
		}

		public Graphics GetGraphics(Image img)
		{
			Graphics graphics = Graphics.FromImage(img);
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			graphics.CompositingQuality = CompositingQuality.HighQuality;
			graphics.InterpolationMode = InterpolationMode.Default;
			graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
			return graphics;
		}

		public void SaveImage2File(string path, Image destImage, int quality, string mimeType = "image/jpeg")
		{
			if (quality <= 0 || quality > 100)
			{
				quality = 95;
			}
			FileInfo fileInfo = new FileInfo(path);
			if (!Directory.Exists(fileInfo.DirectoryName))
			{
				Directory.CreateDirectory(fileInfo.DirectoryName);
			}
			EncoderParameters encoderParameters = new EncoderParameters();
			long[] array = new long[]
			{
				(long)quality
			};
			EncoderParameter encoderParameter = new EncoderParameter(Encoder.Quality, (long)quality);
			encoderParameters.Param[0] = encoderParameter;
			ImageCodecInfo encoder = ImageCodecInfo.GetImageEncoders().FirstOrDefault((ImageCodecInfo ici) => ici.MimeType == mimeType);
			destImage.Save(path, encoder, encoderParameters);
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.label1 = new Label();
			this.label2 = new Label();
			this.num1 = new NumericUpDown();
			((ISupportInitialize)this.num1).BeginInit();
			base.SuspendLayout();
			this.label1.AutoSize = true;
			this.label1.Location = new Point(50, 37);
			this.label1.Name = "label1";
			this.label1.Size = new Size(197, 12);
			this.label1.TabIndex = 1;
			this.label1.Text = "压缩比率(1-100 数字越小图片越小)";
			this.label2.AutoSize = true;
			this.label2.Location = new Point(102, 9);
			this.label2.Name = "label2";
			this.label2.Size = new Size(221, 12);
			this.label2.TabIndex = 2;
			this.label2.Text = "请拖入图片文件夹或图片文件到空白区域";
			this.num1.Location = new Point(253, 33);
			NumericUpDown arg_136_0 = this.num1;
			int[] array = new int[4];
			array[0] = 1;
			arg_136_0.Minimum = new decimal(array);
			this.num1.Name = "num1";
			this.num1.Size = new Size(120, 21);
			this.num1.TabIndex = 3;
			NumericUpDown arg_187_0 = this.num1;
			array = new int[4];
			array[0] = 100;
			arg_187_0.Value = new decimal(array);
			this.num1.ValueChanged += new EventHandler(this.numericUpDown1_ValueChanged);
			this.AllowDrop = true;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.AutoScaleMode = AutoScaleMode.Font;
			base.ClientSize = new Size(399, 158);
			base.Controls.Add(this.num1);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.label1);
			base.MaximizeBox = false;
			this.MaximumSize = new Size(415, 197);
			this.MinimumSize = new Size(415, 197);
			base.Name = "Form1";
			this.Text = "图片压缩";
			base.DragDrop += new DragEventHandler(this.Form1_DragDrop);
			base.DragEnter += new DragEventHandler(this.Form1_DragEnter);
			((ISupportInitialize)this.num1).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
