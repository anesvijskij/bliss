using Meta.ORM.Model;

namespace Meta.ORM.Expressions
{
    public class MetaExpression
    {
        public MetaExpression<bool> And(MetaExpression<bool> andExpression)
        {
            return new MetaExpression<bool>();
        }

        public MetaExpression<bool> Or(MetaExpression<bool> orExpression)
        {
            return new MetaExpression<bool>();
        }

        public MetaExpression<bool> Equal(MetaExpression expression)
        {
            return new MetaExpression<bool>();
        }

        public static MetaExpression Property(string propertyName)
        {
            return new MetaExpression();
        }

        public static MetaExpression Property(IProperty property)
        {
            return new MetaExpression();
        }

        public static MetaExpression<T> Property<T>(IProperty<T> property)
        {
            return new MetaExpression<T>();
        }

        public static MetaExpression Constant(object value)
        {
            return new MetaExpression();
        }

        public static MetaExpression This()
        {
            return new MetaExpression();
        }

        public static MetaExpression Container()
        {
            return new MetaExpression();
        }

        public static MetaExpression<T> Constant<T>(T value)
        {
            return new MetaExpression<T>();
        }

        public MetaExpression<int> StringLength()
        {
            return new MetaExpression<int>();
        }

        public static MetaExpression Parse(string expression)
        {
            return new MetaExpression();
        }

        public static MetaExpression<T> Parse<T>(string expression)
        {
            return new MetaExpression<T>();
        }
    }

    public class MetaExpression<T> : MetaExpression
    {
        //public MetaExpression<bool> And(MetaExpression<bool> andExpression)
        //{
        //    return new MetaExpression<bool>();
        //}

        //public MetaExpression<bool> Or(MetaExpression<bool> orExpression)
        //{
        //    return new MetaExpression<bool>();
        //}

       

        
    }
}