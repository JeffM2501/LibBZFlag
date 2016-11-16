namespace TestClient
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.AuthPannel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.Callsign = new System.Windows.Forms.TextBox();
            this.Motto = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Password = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.AuthButton = new System.Windows.Forms.Button();
            this.ServerList = new System.Windows.Forms.ListBox();
            this.OnlyFilledServers = new System.Windows.Forms.CheckBox();
            this.Join = new System.Windows.Forms.Button();
            this.ServerInfo = new System.Windows.Forms.TextBox();
            this.PlayersList = new System.Windows.Forms.ListBox();
            this.ChatArea = new System.Windows.Forms.TextBox();
            this.SendChatLine = new System.Windows.Forms.TextBox();
            this.SendChat = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.AuthPannel.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.AuthPannel);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.SendChat);
            this.splitContainer1.Panel2.Controls.Add(this.SendChatLine);
            this.splitContainer1.Panel2.Controls.Add(this.ChatArea);
            this.splitContainer1.Panel2.Controls.Add(this.PlayersList);
            this.splitContainer1.Size = new System.Drawing.Size(1061, 621);
            this.splitContainer1.SplitterDistance = 257;
            this.splitContainer1.TabIndex = 0;
            // 
            // AuthPannel
            // 
            this.AuthPannel.Controls.Add(this.ServerInfo);
            this.AuthPannel.Controls.Add(this.Join);
            this.AuthPannel.Controls.Add(this.OnlyFilledServers);
            this.AuthPannel.Controls.Add(this.ServerList);
            this.AuthPannel.Controls.Add(this.AuthButton);
            this.AuthPannel.Controls.Add(this.Password);
            this.AuthPannel.Controls.Add(this.label3);
            this.AuthPannel.Controls.Add(this.Motto);
            this.AuthPannel.Controls.Add(this.label2);
            this.AuthPannel.Controls.Add(this.Callsign);
            this.AuthPannel.Controls.Add(this.label1);
            this.AuthPannel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AuthPannel.Location = new System.Drawing.Point(0, 0);
            this.AuthPannel.Name = "AuthPannel";
            this.AuthPannel.Size = new System.Drawing.Size(257, 621);
            this.AuthPannel.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Callsign";
            // 
            // Callsign
            // 
            this.Callsign.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Callsign.Location = new System.Drawing.Point(72, 10);
            this.Callsign.Name = "Callsign";
            this.Callsign.Size = new System.Drawing.Size(182, 20);
            this.Callsign.TabIndex = 1;
            this.Callsign.TextChanged += new System.EventHandler(this.Callsign_TextChanged);
            // 
            // Motto
            // 
            this.Motto.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Motto.Location = new System.Drawing.Point(72, 36);
            this.Motto.Name = "Motto";
            this.Motto.Size = new System.Drawing.Size(182, 20);
            this.Motto.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Motto";
            // 
            // Password
            // 
            this.Password.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Password.Location = new System.Drawing.Point(72, 62);
            this.Password.Name = "Password";
            this.Password.PasswordChar = '*';
            this.Password.Size = new System.Drawing.Size(182, 20);
            this.Password.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Password";
            // 
            // AuthButton
            // 
            this.AuthButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AuthButton.Location = new System.Drawing.Point(195, 88);
            this.AuthButton.Name = "AuthButton";
            this.AuthButton.Size = new System.Drawing.Size(59, 23);
            this.AuthButton.TabIndex = 6;
            this.AuthButton.Text = "Login";
            this.AuthButton.UseVisualStyleBackColor = true;
            this.AuthButton.Click += new System.EventHandler(this.AuthButton_Click);
            // 
            // ServerList
            // 
            this.ServerList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ServerList.FormattingEnabled = true;
            this.ServerList.Location = new System.Drawing.Point(3, 120);
            this.ServerList.Name = "ServerList";
            this.ServerList.Size = new System.Drawing.Size(251, 368);
            this.ServerList.TabIndex = 7;
            this.ServerList.SelectedIndexChanged += new System.EventHandler(this.ServerList_SelectedIndexChanged);
            this.ServerList.DoubleClick += new System.EventHandler(this.ServerList_DoubleClick);
            // 
            // OnlyFilledServers
            // 
            this.OnlyFilledServers.AutoSize = true;
            this.OnlyFilledServers.Location = new System.Drawing.Point(3, 92);
            this.OnlyFilledServers.Name = "OnlyFilledServers";
            this.OnlyFilledServers.Size = new System.Drawing.Size(178, 17);
            this.OnlyFilledServers.TabIndex = 8;
            this.OnlyFilledServers.Text = "Only Show Servers With Players";
            this.OnlyFilledServers.UseVisualStyleBackColor = true;
            this.OnlyFilledServers.CheckedChanged += new System.EventHandler(this.OnlyFilledServers_CheckedChanged);
            // 
            // Join
            // 
            this.Join.Location = new System.Drawing.Point(179, 586);
            this.Join.Name = "Join";
            this.Join.Size = new System.Drawing.Size(75, 23);
            this.Join.TabIndex = 9;
            this.Join.Text = "Join";
            this.Join.UseVisualStyleBackColor = true;
            this.Join.Click += new System.EventHandler(this.Join_Click);
            // 
            // ServerInfo
            // 
            this.ServerInfo.Location = new System.Drawing.Point(3, 490);
            this.ServerInfo.Multiline = true;
            this.ServerInfo.Name = "ServerInfo";
            this.ServerInfo.ReadOnly = true;
            this.ServerInfo.Size = new System.Drawing.Size(251, 90);
            this.ServerInfo.TabIndex = 10;
            // 
            // PlayersList
            // 
            this.PlayersList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PlayersList.FormattingEnabled = true;
            this.PlayersList.Location = new System.Drawing.Point(549, 5);
            this.PlayersList.Name = "PlayersList";
            this.PlayersList.Size = new System.Drawing.Size(248, 433);
            this.PlayersList.TabIndex = 0;
            // 
            // ChatArea
            // 
            this.ChatArea.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ChatArea.Location = new System.Drawing.Point(4, 446);
            this.ChatArea.Multiline = true;
            this.ChatArea.Name = "ChatArea";
            this.ChatArea.ReadOnly = true;
            this.ChatArea.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ChatArea.Size = new System.Drawing.Size(793, 140);
            this.ChatArea.TabIndex = 1;
            // 
            // SendChatLine
            // 
            this.SendChatLine.Location = new System.Drawing.Point(4, 592);
            this.SendChatLine.Name = "SendChatLine";
            this.SendChatLine.Size = new System.Drawing.Size(734, 20);
            this.SendChatLine.TabIndex = 2;
            this.SendChatLine.TextChanged += new System.EventHandler(this.SendChatLine_TextChanged);
            // 
            // SendChat
            // 
            this.SendChat.Location = new System.Drawing.Point(744, 589);
            this.SendChat.Name = "SendChat";
            this.SendChat.Size = new System.Drawing.Size(44, 23);
            this.SendChat.TabIndex = 3;
            this.SendChat.Text = "Send";
            this.SendChat.UseVisualStyleBackColor = true;
            this.SendChat.Click += new System.EventHandler(this.SendChat_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1061, 621);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "TestClient";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.AuthPannel.ResumeLayout(false);
            this.AuthPannel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel AuthPannel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Motto;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Callsign;
        private System.Windows.Forms.TextBox Password;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox OnlyFilledServers;
        private System.Windows.Forms.ListBox ServerList;
        private System.Windows.Forms.Button AuthButton;
        private System.Windows.Forms.Button Join;
        private System.Windows.Forms.TextBox ServerInfo;
        private System.Windows.Forms.ListBox PlayersList;
        private System.Windows.Forms.TextBox ChatArea;
        private System.Windows.Forms.Button SendChat;
        private System.Windows.Forms.TextBox SendChatLine;
    }
}

