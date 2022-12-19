﻿using DoAn_IE307_N11.Models;
using DoAn_IE307_N11.Services;
using MvvmHelpers;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Transactions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Transaction = DoAn_IE307_N11.Models.Transaction;

namespace DoAn_IE307_N11.ViewModels
{
    public enum TabType 
    {
        Day,
        Week,
        Month,
        Year
    }

    public class TabViewModel : BaseViewModel
    {
        #region Private Members

        private ObservableCollection<TransactionPod> _transactionPods;


        #endregion

        public TabViewModel(string title)
        {
            this.Title = title;
        }

        public bool IsSelected { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        /// <summary>
        /// A positive number represent total income
        /// </summary>
        public int Income => TransactionPods is null ? 0 : TransactionPods.Sum(t => t.Income);

        /// <summary>
        /// A negative number represent total outcome
        /// </summary>
        public int Outcome => TransactionPods is null ? 0 : TransactionPods.Sum(t => t.Outcome);

        /// <summary>
        /// The balance
        /// </summary>
        public int Balance => Income + Outcome;

        /// <summary>
        /// A list contains all transaction in a container such as: day, week, month
        /// </summary>
        public ObservableCollection<TransactionPod> TransactionPods
        {
            get
            {
                if (_transactionPods is null)
                    LoadTransactions();

                return _transactionPods;
            }

            set => _transactionPods = value;
        }

        public bool HaveTransaction => TransactionPods != null && TransactionPods.Count() > 0;

        #region Private Functions

        public TabType TabType { get; set; }

        private async void LoadTransactions()
        {
            List<Models.Transaction> transactions = null;

            using (var httpClient = new HttpClient())
            {
                var ip = DependencyService.Get<ConstantService>().MY_IP;
                var walletId = 1;

                var datas = await httpClient.GetStringAsync($"http://{ip}/moneybook/api/ServiceController/GetTransactionsByWallet?walletId={walletId}");
                var convertedData = JsonConvert.DeserializeObject<List<Models.Transaction>>(datas);

                transactions = convertedData;
            }

            var temp = transactions
                .Where(tran =>
                    tran.Date.Date >= this.StartDate.Date &&
                    tran.Date.Date <= this.EndDate.Date
                )
                .Select(tran => new TransactionViewModel
                {
                    Transaction = tran,
                })
                .ToArray();

            if (temp.Count() <= 0)
                return;

            switch (TabType)
            {
                case TabType.Day:
                    _transactionPods = new ObservableCollection<TransactionPod>();

                    var transactionPod = new TransactionPod
                    {
                        TransactionPodType = TransactionPodType.Day,
                        DateTime = temp.First().Transaction.Date,
                    };

                    transactionPod.Transactions = new ObservableCollection<TransactionViewModel>(temp);

                    TransactionPods.Add(transactionPod);

                    break;
                case TabType.Week:
                    break;
                case TabType.Month:
                    break;
                case TabType.Year:
                    break;
            }
        }

        #endregion
    }
}