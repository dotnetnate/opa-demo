namespace data_generator{

    public interface IObjectPersistenceStrategy
    {
        void Persist(object objectToPersist);
    }


}