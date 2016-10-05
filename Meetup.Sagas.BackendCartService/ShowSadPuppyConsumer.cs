using System;
using System.Threading.Tasks;
using MassTransit;
using Meetup.Sagas.Contracts;

namespace Meetup.Sagas.BackendCartService
{
    public class ShowSadPuppyConsumer : IConsumer<ShowSadPuppy>
    {
        public Task Consume(ConsumeContext<ShowSadPuppy> context)
        {
            var text =
                @"
           ____,'`-, 
      _,--'   ,/::.; 
   ,-'       ,/::,' `---.___        ___,_ 
   |       ,:';:/        ;'\ `; \'`--./ ,-^.;--. 
    |:     ,:';,'         '         `.   ;`   `-. 
    \:.,:::/;/ -:.                   `  | `     `-.
     \:::,'//__.;  ,;  ,  ,  :.`-.   :. |  ;       :. 
      \,',';/ O)^. :'  ;  :   '__` `  :::`.       .:' ) 
       |,'  |\__,: ;      ;  ' / O)`.   :::`; ' ,'
        |`--''            \__,' , ::::(       ,'
           `    ,            `--' ,: :::,'\   ,-' 
            | ,;         ,    ,::'  ,:::   |,'
            |,:        .(          ,:::|   ` 
            ::'_   _   ::         ,::/:| 
           ,',' `-' \   `.      ,:::/,:| 
          | : _ _   | '     ,::,' ::: 
          | \ O`'O  ,',   ,    :,'   ;:: 
           \ `-'`--',:' ,' , ,,'      :: 
            ``:.:.__   ',-','        ::'
    - hrr -      `--.__, ,::.         ::' 
                     |:  ::::.       ::' 
                     |:  ::::::    ,::' 

";
            Console.WriteLine(text);

            return Task.CompletedTask;
        }
    }
}
