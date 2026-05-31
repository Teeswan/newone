import sys
import os
import re
import time
import subprocess
import webbrowser
from google import genai

SESSION_FILE = ".gemini_session"

def get_git_branch():
    try:
        return subprocess.check_output(["git", "branch", "--show-current"], stderr=subprocess.DEVNULL).decode().strip() or "main"
    except:
        return "main"

def check_login_status():
    return os.path.exists(SESSION_FILE) or "GEMINI_API_KEY" in os.environ

def print_dashboard():
    os.system('cls' if os.name == 'nt' else 'clear')
    print(" 🌟 Gemini CLI v0.44.1\n")
    print("    Signed in with Google /auth")
    print("    Plan: Gemini Code Assist for individuals /upgrade\n")
    
    print("\033[93m┌──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┐")
    print("│ Gemini CLI is transitioning to the new Antigravity CLI for Google One and unpaid tier (Gemini Code Assist) users.     │")
    print("│ What's Changing: We are unifying our tools into a single, multi-agent platform called Antigravity.                  │")
    print("│ Gemini CLI will stop serving requests starting June 18th. Please migrate to Antigravity CLI to avoid disruption.     │")
    print("│ To learn more visit: https://goo.gle/gemini-cli-migration                                                             │")
    print("└──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────┘\033[0m\n")
    
    print("Tips for getting started:")
    print("1. Create GEMINI.md files to customize your interactions")
    print("2. /help for more information")
    print("3. Ask coding questions, edit code or run commands")
    print("4. Be specific for the best results\n")
    print("                                                                                                       ? for shortcuts")
    print("Shift+Tab to accept edits")

def print_status_bar():
    workspace = os.getcwd()
    branch = get_git_branch()
    display_path = workspace if len(workspace) < 30 else "..." + workspace[-25:]
    print("\n" + "-" * 118)
    print(f"{'workspace (/directory)':<30}{'branch':<15}{'sandbox':<15}{'/model':<30}{'quota'}")
    print(f"{display_path:<30}{branch:<15}{'no sandbox':<15}{'gemini-1.5-flash':<30}{'95% used'}")
    print("-" * 118)

def scan_workspace():
    """Workspace ထဲက အရေးကြီးတဲ့ Backend ဖိုင်တွေကို လိုက်ရှာဖတ်ပေးသည့် စနစ်"""
    scanned_context = ""
    extensions = ('.cs', '.json', '.md')
    exclude_dirs = ['bin', 'obj', '.git', '.vs']
    
    for root, dirs, files in os.walk('.'):
        dirs[:] = [d for d in dirs if d not in exclude_dirs]
        for file in files:
            if file.endswith(extensions):
                full_path = os.path.join(root, file)
                try:
                    with open(full_path, 'r', encoding='utf-8') as f:
                        scanned_context += f"\n--- FILE: {full_path} ---\n{f.read()[:2000]}\n" # ဖိုင်တစ်ခုချင်းစီရဲ့ ကုဒ် အစိပ်အပိုင်းများကို စုဆောင်းခြင်း
                except:
                    continue
    return scanned_context

def main():
    print_dashboard()
    
    # API Key စစ်ဆေးခြင်း
    api_key = os.environ.get("GEMINI_API_KEY") or os.environ.get("GOOGLE_API_KEY")
    client = None
    
    if api_key:
        try:
            client = genai.Client(api_key=api_key)
        except Exception as e:
            print(f"⚠️ SDK Init Error: {e}")
            client = None

    while True:
        try:
            print_status_bar()
            prompt = input("> Type your message or @path/to/file: ")
            
            if prompt.strip().lower() in ['exit', 'quit']:
                break
            if prompt.strip().lower() == 'clear':
                print_dashboard()
                continue
            if not prompt.strip():
                print_dashboard()
                continue

            print("\n🤖 [Thinking] Asking Gemini Code Assist...")

            # 🛠️ "scan" သို့မဟုတ် "solution" ပါလာခဲ့လျှင် တစ်ခုလုံးကို Scan ဖတ်မည့် Logic
            if "scan" in prompt.lower() or "solution" in prompt.lower():
                print("🔍 [Scanning Workspace] Reading files inside EPMS Solution...")
                context = scan_workspace()
                full_prompt = f"The user wants to scan the workspace. Here is the codebase context:\n{context}\n\nUser Request: {prompt}"
            else:
                # single file handler (@file)
                file_matches = re.findall(r'@([^\s]+)', prompt)
                if file_matches and os.path.exists(file_matches[0]):
                    try:
                        with open(file_matches[0], 'r', encoding='utf-8') as f:
                            full_prompt = f"Context file {file_matches[0]}:\n{f.read()}\n\nRequest: {prompt}"
                    except:
                        full_prompt = prompt
                else:
                    full_prompt = prompt

            if client:
                try:
                    response = client.models.generate_content(
                        model='gemini-1.5-flash',
                        contents=full_prompt,
                    )
                    print(f"\n--- Response ---\n{response.text}")
                except Exception as e:
                    print(f"\n❌ [API Error]: {e}")
            else:
                print("\n⚠️ [CLI Configuration Required]")
                print("တကယ့် Gemini Live Response ရရှိရန်အတွက် 'GEMINI_API_KEY' သတ်မှတ်ပေးဖို့ လိုအပ်နေပါတယ်ဗျာ။")
                print("PowerShell မှာ အောက်ပါအတိုင်း ရိုက်ထည့်ပြီးမှ ပြန်လည်မောင်းနှင်ပေးပါ-")
                print(' -> $env:GEMINI_API_KEY="YOUR_ACTUAL_API_KEY"')
                
        except KeyboardInterrupt:
            break

if __name__ == "__main__":
    main()
