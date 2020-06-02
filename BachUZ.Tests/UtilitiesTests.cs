using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BachUZ.Discord.Utils;

namespace BachUZ.Tests
{
    [TestClass]
    public class UtilitiesTests
    {
        [TestMethod]
        public void TestMessageSplit()
        {
            const string text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla finibus, metus vitae dapibus condimentum, tortor leo rhoncus elit, eget pharetra justo est et diam. Vestibulum ornare rhoncus magna nec eleifend. Donec mattis diam vel justo vestibulum consequat. Curabitur dolor leo, condimentum euismod maximus nec, pretium ut odio. Nunc aliquet eros ac lorem fermentum bibendum. Pellentesque vel leo varius, dignissim arcu at, venenatis erat. Aliquam vitae massa quis lorem lacinia suscipit. Quisque hendrerit felis est, ullamcorper feugiat augue laoreet ac. Sed non congue eros. Curabitur nec felis facilisis sem eleifend luctus. Donec tortor dolor, scelerisque non placerat pulvinar, facilisis fringilla velit. Ut ullamcorper magna sed elit aliquet ultrices. Donec congue fermentum ex, egestas suscipit sem viverra tristique. Quisque eget suscipit sapien.\r\nCurabitur nulla tortor, lacinia vitae volutpat et, ultrices sed arcu. Curabitur tempor ipsum in mi facilisis auctor. Donec id nibh sollicitudin, egestas neque sed, rutrum felis. Pellentesque tincidunt pretium nibh in sollicitudin. Praesent vitae est semper, sodales ex sit amet, euismod risus. Nullam dapibus ultricies eros. Quisque nec velit at tellus luctus placerat.\r\nQuisque sollicitudin mollis efficitur. Duis non orci ac enim facilisis consequat eu ac libero. Maecenas mattis mi ut erat pellentesque, a gravida libero suscipit. Pellentesque interdum quam a eros consectetur porta. Donec elit mauris, porta at efficitur eget, hendrerit sed magna. Curabitur nec nisl orci. Donec at felis id nulla vestibulum rutrum vitae vel massa. Nam commodo nunc magna, a luctus est convallis quis. Integer fringilla erat a erat iaculis pellentesque vel eu libero. Nullam malesuada sagittis turpis, nec pretium quam tempus ac. Proin at imperdiet leo, a mattis felis. Sed cursus elit eu tempus ultricies. Pellentesque suscipit tempus nisi sed sagittis. Ut vel erat est. Etiam sed tortor a arcu volutpat vestibulum ut sed odio.\r\nUt vitae enim ut erat euismod maximus vitae in magna. Vivamus tincidunt nunc non urna egestas, at convallis arcu faucibus. Morbi ac viverra dui. Morbi est nisi, rutrum at justo hendrerit, posuere ullamcorper neque. Etiam in tristique sapien, vel interdum lectus. Mauris tempus malesuada iaculis. Maecenas malesuada feugiat mi eget suscipit. Sed in justo id magna sollicitudin dignissim. Sed at quam ultrices, dictum enim at, vestibulum felis. Sed id elit tellus. Integer vitae lectus velit. Aliquam faucibus mattis turpis, ut tempor nisi. Nulla sit amet urna vitae felis blandit lobortis ut vitae mi. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Sed rhoncus lorem sapien, non blandit ipsum volutpat non. In ultricies, enim in placerat elementum, arcu ex luctus purus, accumsan maximus erat nisl quis leo.";
            var chunks = Utilities.SplitMessage(text);
            Assert.IsTrue(chunks.Count > (text.Length / 2000));
        }
    }
}