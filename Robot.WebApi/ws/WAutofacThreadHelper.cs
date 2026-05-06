using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot.WebApi.ws
{
    public class WAutofacThreadHelper
    {
        private readonly IServiceProvider serviceProvider_;
        public WAutofacThreadHelper(IServiceProvider serviceProvider)
        {
            serviceProvider_ = serviceProvider;
        }
        public IServiceScope BeginNewScope()
        {
            return serviceProvider_.CreateScope();
        }
    }
}
