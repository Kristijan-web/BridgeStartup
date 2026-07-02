using Application;
using Application.Commands;
using Application.Exceptions;
using Application.Queries;

namespace Implementation
{
    public class UseCaseHandler
    {


        IApplicationUser _user;

        public UseCaseHandler(IApplicationUser user)
        {
            _user = user;

        }

        private void IsAuthorized(string useCaseId)
        {

            if (!_user.AllowedUseCases.Contains(useCaseId))
            {
                throw new NotAuthorizedException(useCaseId);
            }


        }

        public void ExecuteCommand<T>(ICommand<T> cmd, T dto)
        {

            IsAuthorized(cmd.Id);

            cmd.Execute(dto);

        }


        public Tresponse ExecuteQuery<Tdata, Tresponse>(IQuery<Tdata, Tresponse> query, Tdata dto)
        {

            IsAuthorized(query.Id);

            return query.Execute(dto);

        }


    }
}
