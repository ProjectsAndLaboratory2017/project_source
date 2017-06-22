using ServerApplicationWPF.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerApplicationWPF
{
    public class OnlineProductManager
    {
        private DataManager dataManager;
        public Thread t { get; private set; }
        private BlockingCollection<string> queue;
        public OnlineProductManager(DataManager dataManager, BlockingCollection<string> queue)
        {
            this.dataManager = dataManager;
            this.queue = queue;
            t = new Thread(new ParameterizedThreadStart(consumer));
            // background threads don't keep app running if other threads terminate
            t.IsBackground = true;
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private void consumer(object obj)
        {
            foreach (string barcode in queue.GetConsumingEnumerable())
            {
                Product product = new ProductFinder().search(barcode);
                if (product == null)
                {
                    product = new Product(null, barcode, "", 0, 0, 0, 0);
                }
                AddProduct addProductWindow = new AddProduct(product, dataManager);
                addProductWindow.ShowDialog();
            }
        }
    }
}
