// This file is part of AlarmWorkflow.
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
        private readonly string _thumpPrint;
        /// <summary>
        /// Creates a new CertificateValidator with a given fingerprint
        /// </summary>
        /// <param name="thumpPrint">The fingerprint for validating</param>
        public CertificateValidator(string thumpPrint)
        {
            _thumpPrint = thumpPrint;
        }

        #region Overrides of X509CertificateValidator

        /// <summary>
        /// Überprüft das X.509-Zertifikat, wenn in einer abgeleiteten Klasse überschrieben. 
        /// </summary>
        /// <param name="certificate">Das <see cref="T:System.Security.Cryptography.X509Certificates.X509Certificate2"/>, das das zu überprüfende X.509-Zertifikat darstellt.</param>
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