﻿// This file is part of AlarmWorkflow.
// 
// AlarmWorkflow is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// AlarmWorkflow is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with AlarmWorkflow.  If not, see <http://www.gnu.org/licenses/>.

using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace AlarmWorkflow.Backend.ServiceContracts.Core
{
    /// <summary>
    /// Validator for the X509 certificates based on the fingerprint of the certificate
    /// </summary>
    public class CertificateValidator : X509CertificateValidator
    {
        #region Field(s)

        private readonly string _thumpPrint;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new CertificateValidator with a given fingerprint
        /// </summary>
        /// <param name="thumpPrint">The fingerprint for validating</param>
        public CertificateValidator(string thumpPrint)
        {
            _thumpPrint = thumpPrint;
        }

        #endregion

        #region Overrides of X509CertificateValidator

        /// <summary>
        /// Checks if the given certificate is valid based on the given fingerprint to the constructor.
        /// </summary>
        /// <param name="certificate">The certificate to get checked.</param>
        /// <exception cref="SecurityTokenValidationException">Throws a <see cref="SecurityTokenValidationException"/> if the certificat is not valid.</exception>
        public override void Validate(X509Certificate2 certificate)
        {
            //The fingerprints could contain a whitespace and be lower case. Change this here.
            if (certificate.Thumbprint != _thumpPrint.Replace(" ", "").ToUpper())
            {
                throw new SecurityTokenValidationException();
            }
        }

        #endregion
    }
}