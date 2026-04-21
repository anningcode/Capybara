#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
QQ邮箱发送工具
作者：Python助手
"""

import smtplib
import argparse
from email.mime.text import MIMEText
from email.mime.multipart import MIMEMultipart
from email.header import Header
import sys

class QQMailTool:
    def __init__(self, email, auth_code):
        self.email = email
        self.auth_code = auth_code
        
    def send(self, to, subject, content, html=False, attachments=None):
        """
        发送邮件
        
        Args:
            to: 收件人（单个字符串或列表）
            subject: 主题
            content: 内容
            html: 是否为HTML格式
            attachments: 附件路径列表
        """
        if isinstance(to, str):
            to = [to]
        
        # 创建邮件
        if attachments:
            msg = MIMEMultipart()
        else:
            msg = MIMEText(content, 'html' if html else 'plain', 'utf-8')
        
        # 修复From头格式 - 只使用邮箱地址
        msg['From'] = self.email
        msg['To'] = ', '.join(to)
        msg['Subject'] = Header(subject, 'utf-8')
        
        # 如果有附件
        if attachments:
            # 添加正文
            msg.attach(MIMEText(content, 'html' if html else 'plain', 'utf-8'))
            
            # 添加附件
            for file_path in attachments:
                with open(file_path, 'rb') as f:
                    from email.mime.base import MIMEBase
                    from email import encoders
                    
                    part = MIMEBase('application', 'octet-stream')
                    part.set_payload(f.read())
                    encoders.encode_base64(part)
                    part.add_header('Content-Disposition', 
                                  f'attachment; filename="{file_path}"')
                    msg.attach(part)
        
        # 发送
        try:
            with smtplib.SMTP_SSL("smtp.qq.com", 465) as server:
                server.login(self.email, self.auth_code)
                server.sendmail(self.email, to, msg.as_string())

            print(f"[success] 邮件发送成功到 {len(to)} 位收件人")
            return True
        except Exception as e:
            print(f"[fail] 发送失败: {e}")
            return False

def main():
    # 设置标准输出的编码为utf-8
    sys.stdout.reconfigure(encoding='utf-8')
    
    parser = argparse.ArgumentParser(description="QQ邮箱发送工具")
    parser.add_argument("--email", required=True, help="QQ邮箱地址")
    parser.add_argument("--auth-code", required=True, help="QQ邮箱授权码")
    parser.add_argument("--to", required=True, help="收件人邮箱")
    parser.add_argument("--subject", required=True, help="邮件主题")
    parser.add_argument("--content", required=True, help="邮件内容")
    parser.add_argument("--html", action="store_true", help="使用HTML格式")
    parser.add_argument("--attach", nargs="+", help="附件路径")
    
    args = parser.parse_args()
    
    # 创建发送工具
    tool = QQMailTool(args.email, args.auth_code)
    
    # 发送邮件
    tool.send(
        to=args.to,
        subject=args.subject,
        content=args.content,
        html=args.html,
        attachments=args.attach
    )

if __name__ == "__main__":
    main()