﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace ClinicArrivals.Models
{
    public class SaveTemplatesCommand : ICommand
    {
        IArrivalsLocalStorage _storage;
        bool processing;
        public SaveTemplatesCommand(IArrivalsLocalStorage storage)
        {
            _storage = storage;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (processing)
                return false;
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is IEnumerable<MessageTemplate> templates)
            {
                Boolean ok = true;
                foreach (var template in templates)
                {
                    string error;
                    if (!MessageTemplate.IsValid(template.MessageType, template.Template, out error))
                    {
                        ok = false;
                        MessageBoxButtons buttons = MessageBoxButtons.OK;
                        MessageBox.Show("Template for " + template.MessageType + " is invalid: " + error, "Template Error", buttons, MessageBoxIcon.Error);
                    }
                }
                processing = true;
                try
                {
                    if (ok)
                    {
                        _storage.SaveTemplates(templates);
                    }
                }
                catch (Exception ex)
                {
                    new NLog.LogFactory().GetLogger("ClinicArrivals").Error("Exception Saving Templates: " + ex.Message);
                }
                finally
                {
                    processing = false;
                    CanExecuteChanged?.Invoke(parameter, new EventArgs());
                }
            }
        }
    }

    public class ReloadTemplatesCommand : ICommand
    {
        IArrivalsLocalStorage _storage;
        bool processing;
        public ReloadTemplatesCommand(IArrivalsLocalStorage storage)
        {
            _storage = storage;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (processing)
                return false;
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is ObservableCollection<MessageTemplate> templates)
            {
                processing = true;
                try
                {
                    var loadedTemplates = _storage.LoadTemplates().GetAwaiter().GetResult();
                    templates.Clear();
                    foreach (var template in loadedTemplates)
                        templates.Add(template);
                }
                catch (Exception ex)
                {
                    new NLog.LogFactory().GetLogger("ClinicArrivals").Error("Exception Loading Templates: " + ex.Message);
                }
                finally
                {
                    processing = false;
                    CanExecuteChanged?.Invoke(parameter, new EventArgs());
                }
            }
        }
    }

    public class SeeTemplateDocumentationCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            System.Diagnostics.Process.Start("https://github.com/grahamegrieve/ClinicArrivals/blob/master/documentation/Templates.md");
        }
    }

    public class ClearUnproccessedMessagesCommand : ICommand
    {
        IArrivalsLocalStorage _storage;
        bool processing;
        public ClearUnproccessedMessagesCommand(IArrivalsLocalStorage storage)
        {
            _storage = storage;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (processing)
                return false;
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is ObservableCollection<SmsMessage> messages)
            {
                processing = true;
                try
                {
                    messages.Clear();
                    _storage.ClearUnprocessableMessages();
                }
                catch (Exception ex)
                {
                    new NLog.LogFactory().GetLogger("ClinicArrivals").Error("Exception Clearing Messages: " + ex.Message);
                }
                finally
                {
                    processing = false;
                    CanExecuteChanged?.Invoke(parameter, new EventArgs());
                }
            }
        }
    }

    
}

