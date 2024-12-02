class UserAuthentication {
  constructor(
    public id: number,
    public username: string,
    public password: string,
    public passwordExpiration: string,
    public role: number
  ) {}
}

export default UserAuthentication;
