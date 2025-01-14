﻿using Blish_HUD.Input.WinApi;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Blish_HUD.Input {

    internal abstract class WinApiInputHookManager<THandlerDelegate> {

        private static readonly Logger Logger = Logger.GetLogger<WinApiMouseHookManager>();

        private readonly HookExtern.HookCallbackDelegate hookProc; // Store the callback delegate, otherwise it might get garbage collected
        private          IntPtr                          hook;


        public WinApiInputHookManager() { hookProc = HookCallback; }


        protected abstract HookType HookType { get; }

        protected IList<THandlerDelegate> Handlers { get; } = new SynchronizedCollection<THandlerDelegate>();


        public virtual bool EnableHook() {
            if (hook != IntPtr.Zero) return true;

            Logger.Debug("Enabling");

            hook = HookExtern.SetWindowsHookEx(this.HookType, hookProc, IntPtr.Zero, 0);
            if (hook == IntPtr.Zero) {
                int error = Marshal.GetLastWin32Error();
                Logger.Warn($"SetWindowsHookEx failed with code {error}");
            }
            return hook != IntPtr.Zero;
        }

        public virtual void DisableHook() {
            if (hook == IntPtr.Zero) return;

            Logger.Debug("Disabling");

            if (!HookExtern.UnhookWindowsHookEx(hook)) {
                int error = Marshal.GetLastWin32Error();
                Logger.Warn($"UnhookWindowsHookEx failed with code {error}");
            }
            hook = IntPtr.Zero;
        }

        public virtual void RegisterHandler(THandlerDelegate handleInputCallback) { this.Handlers.Add(handleInputCallback); }

        public virtual void UnregisterHandler(THandlerDelegate handleInputCallback) { this.Handlers.Remove(handleInputCallback); }

        protected abstract int HookCallback(int nCode, IntPtr wParam, IntPtr lParam);

    }

}
