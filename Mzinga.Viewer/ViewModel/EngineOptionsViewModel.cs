﻿// 
// EngineOptionsViewModel.cs
//  
// Author:
//       Jon Thysell <thysell@gmail.com>
// 
// Copyright (c) 2018 Jon Thysell <http://jonthysell.com>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Mzinga.Viewer.Resources;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Mzinga.Viewer.ViewModel
{
    public class EngineOptionsViewModel : ViewModelBase
    {
        public string Title
        {
            get
            {
                return Strings.EngineOptionsTitle;
            }
        }

        public ObservableCollection<ObservableEngineOption> Options
        {
            get
            {
                return _options;
            }
        }
        private ObservableCollection<ObservableEngineOption> _options;

        public RelayCommand Accept
        {
            get
            {
                return _accept ?? (_accept = new RelayCommand(() =>
                {
                    try
                    {
                        Accepted = true;
                        RequestClose?.Invoke(this, null);
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
                }));
            }
        }
        private RelayCommand _accept = null;

        public RelayCommand Reject
        {
            get
            {
                return _reject ?? (_reject = new RelayCommand(() =>
                {
                    try
                    {
                        Accepted = false;
                        RequestClose?.Invoke(this, null);
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
                }));
            }
        }
        private RelayCommand _reject = null;

        public RelayCommand Reset
        {
            get
            {
                return _reset ?? (_reset = new RelayCommand(() =>
                {
                    try
                    {
                        LoadOptions(true);
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.HandleException(ex);
                    }
                }));
            }
        }
        private RelayCommand _reset = null;

        private EngineOptions _originalOptions;

        public bool Accepted { get; private set; }

        public event EventHandler RequestClose;

        public Action<IDictionary<string, string>> Callback { get; private set; }

        public EngineOptionsViewModel(EngineOptions options = null, Action<IDictionary<string, string>> callback = null)
        {
            _originalOptions =  null != options ? options.Clone() : new EngineOptions();

            LoadOptions(false);

            Accepted = false;
            Callback = callback;
        }

        private void LoadOptions(bool notify)
        {
            _options = new ObservableCollection<ObservableEngineOption>();

            foreach (EngineOption eo in _originalOptions)
            {
                if (eo is BooleanEngineOption)
                {
                    _options.Add(new ObservableBooleanEngineOption((BooleanEngineOption)eo));
                }
                else if (eo is IntegerEngineOption)
                {
                    _options.Add(new ObservableIntegerEngineOption((IntegerEngineOption)eo));
                }
                else if (eo is DoubleEngineOption)
                {
                    _options.Add(new ObservableDoubleEngineOption((DoubleEngineOption)eo));
                }
                else if (eo is EnumEngineOption)
                {
                    _options.Add(new ObservableEnumEngineOption((EnumEngineOption)eo));
                }
            }

            if (notify)
            {
                RaisePropertyChanged("Options");
            }
        }

        public void ProcessClose()
        {
            if (null != Callback && Accepted)
            {
                Dictionary<string, string> changedOptions = new Dictionary<string, string>();

                foreach (ObservableEngineOption oeo in Options)
                {
                    if (oeo is ObservableBooleanEngineOption)
                    {
                        if (((BooleanEngineOption)_originalOptions[oeo.Key]).Value != ((ObservableBooleanEngineOption)oeo).Value)
                        {
                            changedOptions.Add(oeo.Key, ((ObservableBooleanEngineOption)oeo).Value.ToString());
                        }
                    }
                    else if (oeo is ObservableIntegerEngineOption)
                    {
                        if (((IntegerEngineOption)_originalOptions[oeo.Key]).Value != ((ObservableIntegerEngineOption)oeo).Value)
                        {
                            changedOptions.Add(oeo.Key, ((ObservableIntegerEngineOption)oeo).Value.ToString());
                        }
                    }
                    else if (oeo is ObservableDoubleEngineOption)
                    {
                        if (((DoubleEngineOption)_originalOptions[oeo.Key]).Value != ((ObservableDoubleEngineOption)oeo).Value)
                        {
                            changedOptions.Add(oeo.Key, ((ObservableDoubleEngineOption)oeo).Value.ToString());
                        }
                    }
                    else if (oeo is ObservableEnumEngineOption)
                    {
                        if (((EnumEngineOption)_originalOptions[oeo.Key]).Value != ((ObservableEnumEngineOption)oeo).Value)
                        {
                            changedOptions.Add(oeo.Key, ((ObservableEnumEngineOption)oeo).Value);
                        }
                    }
                }

                Callback(changedOptions);
            }
        }
    }
}
